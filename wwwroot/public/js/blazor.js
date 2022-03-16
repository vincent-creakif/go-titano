async function connectionDown(options) {
    console.log("Connection DOWN!");
    for (let i = 0; i < options.maxRetries; i++) {
        await this.delay(options.retryIntervalMilliseconds);

        if (this.isDisposed) {
            break;
        }

        try {
            const result = await window.Blazor.reconnect();
            if (result) {
                // Reconnected!
                return;
            } else {
                console.error("Server Rejected");
            }
        }
        catch {
        }
    }

    location.reload();
}

function delay(durationInMs) {
    return new Promise(resolve => setTimeout(resolve, durationInMs));
}

function connectionUp(e) {
    console.log("Connection UP!");
}

window.Blazor.start({
    reconnectionOptions: {
        maxRetries: 30,
        retryIntervalMilliseconds: 500,
    },
    reconnectionHandler: {
        onConnectionDown: e => connectionDown(e),
        onConnectionUp: e => connectionUp(e)
    }
});