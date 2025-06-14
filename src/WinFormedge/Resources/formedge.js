(function (window) {
    const WINDOW_COMMAND_ATTR_NAME = `app-command`;
    const FORMEDGE_MESSAGE_PASSCODE = `{{FORMEDGE_MESSAGE_PASSCODE}}`;
    const WINFORMEDGE_VERSION = `{{WINFORMEDGE_VERSION}}`;
    const HAS_TITLE_BAR = (`{{HAS_TITLE_BAR}}` === "true");

    function postMessage(message) {

        if (window.chrome?.webview?.postMessage && message.message) {
            window.chrome.webview.postMessage(message);
        }
        else {
            console.error("[window.chrome.webview.postMessage] is not supported in this environment.");
        }
    }

    function raiseHostWindowEvent(eventName, detail) {
        window.dispatchEvent(new CustomEvent(eventName, {
            detail: detail,
        }));
    }

    window["formedgeVersion"] = WINFORMEDGE_VERSION;

    window.moveTo = (x, y) => {
        postMessage({
            passcode: FORMEDGE_MESSAGE_PASSCODE,
            message: "FormedgeWindowMoveTo",
            x,
            y
        });
    }

    window.resizeTo = (width, height) => {
        postMessage({
            passcode: FORMEDGE_MESSAGE_PASSCODE,
            message: "FormedgeWindowResizeTo",
            width,
            height
        });
    }

    window.moveBy = (dx, dy) => {
        postMessage({
            passcode: FORMEDGE_MESSAGE_PASSCODE,
            message: "FormedgeWindowMoveBy",
            dx,
            dy
        });
    }

    window.resizeBy = (dx, dy) => {
        postMessage({
            passcode: FORMEDGE_MESSAGE_PASSCODE,
            message: "FormedgeWindowResizeBy",
            dx,
            dy
        });
    }

    function onFormedgeNotifyWindowActivated(data) {
        if (data.state === undefined) return;

        const { state } = data;
        const htmlEl = document.querySelector("html");

        if (state) {
            raiseHostWindowEvent("windowactivated", {});
            htmlEl?.classList.add("window--activated");
            htmlEl?.classList.remove("window--deactivated");
        }
        else {
            raiseHostWindowEvent("windowdeactivate", {});
            htmlEl?.classList.remove("window--activated");
            htmlEl?.classList.add("window--deactivated");
        }



    }

    function onFormedgeNotifyWindowStateChange(data) {
        const { state } = data;
        if (!state) return;

        raiseHostWindowEvent("windowstatechange", { state });

        const htmlEl = document.querySelector("html");

        htmlEl?.classList.remove("window--maximized", "window--minimized", "window--fullscreen");

        switch (state) {
            case "maximized":
                htmlEl?.classList.add("window--maximized");
                break;
            case "minimized":
                htmlEl?.classList.add("window--minimized");
                break;
            case "fullscreen":
                htmlEl?.classList.add("window--fullscreen");
                break;
        }
    }

    function onFormedgeNotifyWindowResize(data) {
        const { x, y, width, height } = data;
        raiseHostWindowEvent("windowresize", { x, y, width, height });
    }

    function onFormedgeNotifyWindowMove(data) {
        const { x, y, screenX, screenY } = data;
        raiseHostWindowEvent("windowmove", { x, y, screenX, screenY });
    }

    function onLoad() {
        const htmlEl = document.querySelector("html");

        window.addEventListener("click", (e) => {
            const button = e.button;

            if (button === 0) {
                let srcElement = e.target;

                while (srcElement && !srcElement.hasAttribute(WINDOW_COMMAND_ATTR_NAME)) {
                    srcElement = srcElement.parentElement;
                }

                if (srcElement) {
                    const command = srcElement.getAttribute(WINDOW_COMMAND_ATTR_NAME)?.toLowerCase();
                    postMessage({
                        passcode: FORMEDGE_MESSAGE_PASSCODE,
                        message: "FormedgeWindowCommand",
                        command: command,
                    });
                }
            }
        });

        //if (IS_SNAP_LAYOUTS_ENABLED) {

        //    let isMaximizeAreaMouseOver = false;

        //    window.addEventListener("mousemove", (e) => {
        //        let srcElement = e.target;

        //        while (srcElement && !srcElement.hasAttribute(WINDOW_COMMAND_ATTR_NAME)) {
        //            srcElement = srcElement.parentElement;
        //        }

        //        if (srcElement && srcElement.getAttribute(WINDOW_COMMAND_ATTR_NAME)?.toLowerCase() === "maximize") {
        //            isMaximizeAreaMouseOver = true;
        //            postMessage({
        //                passcode: FORMEDGE_MESSAGE_PASSCODE,
        //                message: "FormedgeWindowSnapLayoutsRequired",
        //                status: isMaximizeAreaMouseOver
        //            });

        //        }
        //        else {
        //            if (isMaximizeAreaMouseOver) {
        //                isMaximizeAreaMouseOver = false;
        //                postMessage({
        //                    passcode: FORMEDGE_MESSAGE_PASSCODE,
        //                    message: "FormedgeWindowSnapLayoutsRequired",
        //                    status: isMaximizeAreaMouseOver
        //                });
        //            }

        //        }

        //    });
        //}

        if (window.chrome?.webview?.addEventListener) {
            window.chrome.webview.addEventListener("message", (e) => {
                const { passcode, message } = e.data;
                if (passcode !== FORMEDGE_MESSAGE_PASSCODE) {
                    return;
                }
                switch (message) {
                    case "FormedgeNotifyWindowStateChange":
                        onFormedgeNotifyWindowStateChange(e.data);
                        break;
                    case "FormedgeNotifyWindowResize":
                        onFormedgeNotifyWindowResize(e.data);
                        break;
                    case "FormedgeNotifyWindowMove":
                        onFormedgeNotifyWindowMove(e.data);
                        break;
                    case "FormedgeNotifyWindowActivated":
                        onFormedgeNotifyWindowActivated(e.data);
                        break;
                }

            });
        }


        onFormedgeNotifyWindowActivated({
            passcode: FORMEDGE_MESSAGE_PASSCODE,
            message: "FormedgeNotifyWindowActivated",
            state: true
        });

        if (HAS_TITLE_BAR) {
            htmlEl?.classList.add("window__titlbar--shown");
            htmlEl?.classList.remove("window__titlbar--hidden");

        }
        else {
            htmlEl?.classList.add("window__titlbar--hidden");
            htmlEl?.classList.remove("window__titlbar--shown");
        }
    }


    if (document.readyState === "loading") {
        window.addEventListener("load", () => {
            onLoad();
        });
    }
    else {
        onLoad();
    }




    function getHostWindow() {
        if (!window.chrome?.webview?.hostObjects?.sync?.hostWindow) return;
        return window.chrome?.webview?.hostObjects?.sync?.hostWindow
    }

    function hostWindowMinimize() {
        const win = getHostWindow();
        if (!win) return;

        win.Minimize();
    }

    function hostWindowMaximize() {
        const win = getHostWindow();
        if (!win) return;
        win.Maximize();
    }

    function hostWindowRestore() {
        const win = getHostWindow();
        if (!win) return;
        win.Restore();
    }
    function hostWindowFullscreen() {
        const win = getHostWindow();
        if (!win) return;
        win.Fullscreen();
    }
    function hostWindowToggleFullscreen() {
        const win = getHostWindow();
        if (!win) return;
        win.ToggleFullscreen();
    }
    function hostWindowClose() {
        const win = getHostWindow();
        if (!win) return;
        win.Close();
    }
    function hostWindowActivate() {
        const win = getHostWindow();
        if (!win) return;
        win.Activate();
    }


    function getHostWindowActivated() {
        const win = getHostWindow();
        if (!win) return;
        return win.Activated;
    }

    function getHostWindowState() {
        const win = getHostWindow();
        if (!win) return;
        return win.WindowState;
    }

    function getHostWindowHasTitleBar() {
        const win = getHostWindow();
        if (!win) return;
        return win.HasTitleBar;
    }



    class HostWindow {

        constructor() {
            setInterval(() => {


                const state = hostWindow?.activated;

                if (state === null || state === undefined) return;

                const htmlEl = document.querySelector("html");

                if (state) {
                    htmlEl?.classList.add("window--activated");
                    htmlEl?.classList.remove("window--deactivated");
                }
                else {
                    htmlEl?.classList.remove("window--activated");
                    htmlEl?.classList.add("window--deactivated");
                }

            }, 300);
        }

        get activated() {
            return getHostWindowActivated();
        }

        get hasTitleBar() {
            return getHostWindowHasTitleBar();
        }

        get windowState() {
            return getHostWindowState();
        }

        get left() {
            const win = getHostWindow();
            if (!win) return 0;
            return win.Left;
        }

        set left(value) {
            const win = getHostWindow();
            if (!win) return;
            win.Left = value;
        }

        get top() {
            const win = getHostWindow();
            if (!win) return 0;
            return win.Top;

        }

        set top(value) {
            const win = getHostWindow();
            if (!win) return;
            win.Top = value;

        }

        get width() {
            const win = getHostWindow();
            if (!win) return 0;
            return win.Width;

        }

        set width(value) {
            const win = getHostWindow();
            if (!win) return;
            win.Width = value;

        }

        get height() {
            const win = getHostWindow();
            if (!win) return 0;
            return win.Height;

        }

        set height(value) {
            const win = getHostWindow();
            if (!win) return;
            win.Height = value;

        }





        activate() {
            hostWindowActivate();
        }

        minimize() {
            hostWindowMinimize();
        }

        maximize() {
            hostWindowMaximize();
        }

        restore() {
            hostWindowRestore();
        }

        fullscreen() {
            hostWindowFullscreen();
        }

        toggleFullscreen() {
            hostWindowToggleFullscreen();
        }

        close() {
            hostWindowClose();
        }
    }

    function getWinFormedgeVersion() {
        const win = getHostWindow();
        if (!win) return;
        return win.FormedgeVersion;
    }

    function getChromiumVersion() {
        const win = getHostWindow();
        if (!win) return;
        return win.ChromiumVersion;
    }

    class Formedge {

        version = {
            get Formedge() {
                return getWinFormedgeVersion();
            },
            get Chromium() {
                return getChromiumVersion();
            }
        }
    }

    window["formedge"] = new Formedge();
    window["hostWindow"] = new HostWindow();

})(window)