let _web3 = new Web3();
let _dotNetObjectRef;

let _statsCheckInterval;
let _statsCheckIntervalDelayInSec = 10;

let _rebaseCountdownInterval;
let _tokenAddress;
let _walletAddress;

const _tokenAbi =
    [
        // balanceOf
        {
            'constant': true,
            'inputs': [{ 'name': '_owner', 'type': 'address' }],
            'name': 'balanceOf',
            'outputs': [{ 'name': 'balance', 'type': 'uint256' }],
            'type': 'function'
        },
        // decimals
        {
            'constant': true,
            'inputs': [],
            'name': 'decimals',
            'outputs': [{ 'name': '', 'type': 'uint8' }],
            'type': 'function'
        }
    ];

async function connectWalletAsync(walletName, tokenAddress, dotNetObjectRef) {
    _tokenAddress = tokenAddress
    _dotNetObjectRef = dotNetObjectRef

    try {
        if (walletName === "metamask") {
            if (await checkMetaMaskAsync()) {
                _web3 = new Web3(window.ethereum)
                try {
                    await setConnectedWalletAsync(walletName)
                    await setRebaseCountDownAsync()
                    await getTitanoStatsAsync()
                    await getInitialBalanceAsync()

                    _statsCheckInterval = setInterval(
                        async () => getTitanoStatsAsync(), _statsCheckIntervalDelayInSec * 1000)

                    _rebaseCountdownInterval = setInterval(
                        async () => setRebaseCountDownAsync(), 1000)
                }
                catch {
                }
            }
        }
    }
    catch (e) {
        console.error(e)
    }
}

function resetApp() {
    clearInterval(_statsCheckInterval)
    clearInterval(_rebaseCountdownInterval)
    _tokenAddress = null
    _walletAddress = null
    _dotNetObjectRef = null
}

function getTimezoneOffset() {
    return new Date().getTimezoneOffset()
}

async function getTitanoStatsAsync() {
    await getTitanoPriceAsync()
    await getTitanoBalancesAsync()
    await getTitanoEarningsAsync()
}

async function getTitanoBalancesAsync() {
    const accounts = await _web3.eth.getAccounts()
    _walletAddress = accounts[0]

    const tokenInst = new _web3.eth.Contract(_tokenAbi, _tokenAddress)
    const balance = await tokenInst.methods.balanceOf(_walletAddress).call()

    await _dotNetObjectRef.invokeMethodAsync(
        "SetTitanoBalancesAsync",
        _web3.utils.fromWei(balance))
}

async function getTitanoPriceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoPriceAsync")
}

async function getTitanoEarningsAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoEarningsAsync")
}

async function setConnectedWalletAsync(wallet) {
    await _dotNetObjectRef.invokeMethodAsync("SetConnectedWalletAsync", wallet);
}

async function setRebaseCountDownAsync() {
    await _dotNetObjectRef.invokeMethodAsync("SetRebaseCountdownAsync");
}

async function getInitialBalanceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetInitialBalanceAsync", _walletAddress)
}

async function checkMetaMaskAsync() {
    // MetaMask is not installed
    if (!window.ethereum) {
        return false
    }

    // Listen to accountsChanged event to handle disconnection
    ethereum.on("accountsChanged", onMetaMaskAccountsChanged)

    if (!ethereum.selectedAddress || ethereum.selectedAddress == null) {
        try {
            await window.ethereum.request({ method: "eth_requestAccounts" })
            return true
        }
        catch {
            return false
        }
    }

    return true
}

async function onMetaMaskAccountsChanged(accounts) {
    if (accounts.length == 0) {
        ethereum.removeListener("accountsChanged", onMetaMaskAccountsChanged)
        await _dotNetObjectRef.invokeMethodAsync("DisconnectWalletAsync")
    }
}