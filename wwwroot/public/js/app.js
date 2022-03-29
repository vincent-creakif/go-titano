const PriceCheckIntervalDelayInSec = 10
// Check current balance when rebase countdown value in seconds is equal to
const CheckCurrentBalanceWhenRebaseCountdownEquals = (29 * 60) // when rebase countdown = 29:00

let _dotNetObjectRef
let _priceCheckInterval
let _rebaseCountdownInterval

async function initAppAsync(dotNetObjectRef) {
    _dotNetObjectRef = dotNetObjectRef

    await getTitanoPriceAsync()
    await getCurrentBalanceAsync()
    await getInitialBalanceAsync()
    await setRebaseCountDownAsync()

    _priceCheckInterval = setInterval(
        async () => getTitanoPriceAsync(), PriceCheckIntervalDelayInSec * 1000)

    _rebaseCountdownInterval = setInterval(
        async () => setRebaseCountDownAsync(), 1000)
}

function resetAppAsync() {
    clearInterval(_priceCheckInterval)
    clearInterval(_rebaseCountdownInterval)
    _dotNetObjectRef = null
}

function getTimezoneOffset() {
    return new Date().getTimezoneOffset()
}

async function getCurrentBalanceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoBalanceAsync")
}

async function getTitanoPriceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoPriceAsync")
}

async function getTitanoHoldersAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetTitanoHoldersAsync")
}

async function getInitialBalanceAsync() {
    await _dotNetObjectRef.invokeMethodAsync("GetInitialBalanceAsync")
}

async function setRebaseCountDownAsync() {
    var countdownValue = await _dotNetObjectRef.invokeMethodAsync("SetRebaseCountdownAsync")
    if (countdownValue == CheckCurrentBalanceWhenRebaseCountdownEquals) {
        await getCurrentBalanceAsync()
    }
}