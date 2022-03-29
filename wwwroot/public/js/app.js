let _dotNetObjectRef;

const _priceCheckIntervalDelayInSec = 10;
let _priceCheckInterval;

const _currentBalanceCheckIntervalInSec = 1200; // every 20 minutes
let _currentBalanceCheckInterval;

let _rebaseCountdownInterval;

async function initAppAsync(dotNetObjectRef) {
    _dotNetObjectRef = dotNetObjectRef

    await getTitanoPriceAsync()
    await getCurrentBalanceAsync()
    await getInitialBalanceAsync()
    await setRebaseCountDownAsync()

    _currentBalanceCheckInterval = setInterval(
        async () => getCurrentBalanceAsync(), _currentBalanceCheckIntervalInSec * 1000)

    _priceCheckInterval = setInterval(
        async () => getTitanoPriceAsync(), _priceCheckIntervalDelayInSec * 1000)

    _rebaseCountdownInterval = setInterval(
        async () => setRebaseCountDownAsync(), 1000)
}

function resetAppAsync() {
    clearInterval(_currentBalanceCheckInterval)
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
    await _dotNetObjectRef.invokeMethodAsync("SetRebaseCountdownAsync");
}