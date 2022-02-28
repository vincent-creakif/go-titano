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

async function initAppAsync(tokenAddress, dotNetObjectRef)
{
    _tokenAddress = tokenAddress
    _dotNetObjectRef = dotNetObjectRef    

    if (typeof window.ethereum !== "undefined")
    {
        _web3 = new Web3(window.ethereum)
        try
        {
            await window.ethereum.request({ method: "eth_requestAccounts" })
            await setRebaseCountDownAsync()
            await getTitanoStatsAsync()
            await getInitialBalanceAsync()

            _statsCheckInterval = setInterval(
                async () => getTitanoStatsAsync(), _statsCheckIntervalDelayInSec * 1000)

            _rebaseCountdownInterval = setInterval(
                async () => setRebaseCountDownAsync(), 1000)
        }
        catch (e)
        {
            return false
        }
    }
}

function getTimezoneOffset()
{
    return new Date().getTimezoneOffset()
}

async function getTitanoStatsAsync()
{
    await getTitanoPriceAsync()
    await getTitanoBalancesAsync()
    await getTitanoEarningsAsync()
}

async function getTitanoBalancesAsync()
{
    const accounts = await _web3.eth.getAccounts()
    _walletAddress = accounts[0]

    const tokenInst = new _web3.eth.Contract(_tokenAbi, _tokenAddress)
    const balance = await tokenInst.methods.balanceOf(_walletAddress).call()

    await _dotNetObjectRef.invokeMethodAsync(
        "SetTitanoBalancesAsync",
        _web3.utils.fromWei(balance))
}

async function getTitanoPriceAsync()
{
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoPriceAsync")
}

async function getTitanoEarningsAsync()
{
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoEarningsAsync")
}

async function setRebaseCountDownAsync()
{
    await _dotNetObjectRef.invokeMethodAsync("SetRebaseCountdownAsync");
}

async function getInitialBalanceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetInitialBalanceAsync", _walletAddress)
}