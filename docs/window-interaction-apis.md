# Window Interaction APIs

In WinFormedge, the framework provides a series of window interaction APIs to help you implement custom title bar buttons, window dragging, window resizing, and other functionality. Some of these APIs are based on HTML and CSS properties, while others are based on JavaScript interfaces provided by WinFormedge. You can refer to the following content to learn how to use these APIs to enhance your application's user experience.

## Moving Windows

To implement window dragging functionality, you can use the CSS `-webkit-app-region` property. Areas with this property set to `drag` can be used to drag the window, while areas set to `no-drag` will not respond to drag operations.

```css
.title-bar {
  -webkit-app-region: drag; /* Allow window dragging */
}
.title-bar .close-button {
  -webkit-app-region: no-drag; /* Disallow window dragging */
}
```

In the example above, elements with the `.title-bar` class can be used to drag the window, while elements with the `.close-button` class will not respond to drag operations.

## Controlling Window Behavior

You can use HTML attributes and JavaScript interfaces provided by WinFormedge to implement custom title bar button functionality. For example, you can create custom close, minimize, and maximize buttons and bind corresponding event handlers.

WinFormedge provides interfaces for **closing the window**, **minimizing the window**, **maximizing/restoring the window**, and **fullscreen mode**. You can call these interfaces using HTML attributes or JavaScript.

### HTML Attributes

You can also directly use the `app-command` attribute in HTML elements to bind window control commands. Here's an example showing how to create custom title bar buttons:

```html
<div>
  <button
    class="fullscreen"
    app-command="fullscreen"
    title="Fullscreen"
  ></button>
  <button class="minimize" app-command="minimize" title="Minimize"></button>
  <button class="maximize" app-command="maximize" title="Maximize"></button>
  <button class="close" app-command="close" title="Close"></button>
</div>
```

Additionally, `WinFormedge` provides some CSS class names to indicate the current state of the window. These class names are automatically added to the `html` element, and you can use them to adjust the frontend display for different window states:

| Class Name                 | Description                    |
| -------------------------- | ------------------------------ |
| window--activated          | Window is in activated state   |
| window--deactivated        | Window is in deactivated state |
| window--maximized          | Window is in maximized state   |
| window--minimized          | Window is in minimized state   |
| window--fullscreen         | Window is in fullscreen state  |
| window\_\_titlebar--shown  | Title bar is shown             |
| window\_\_titlebar--hidden | Title bar is hidden            |

The following example demonstrates how to use the `window--maximized` class name to control the display and hiding of maximize and restore buttons:

CSS styles:

```css
.restore {
  display: none; /* Hide restore button by default */
}
.window--maximized .maximize {
  display: none; /* Hide maximize button when window is maximized */
}
.window--maximized .restore {
  display: block; /* Show restore button when window is maximized */
}
```

HTML code:

```html
<div>
  <!-- ... -->
  <button class="maximize" app-command="maximize" title="Maximize"></button>
  <button class="restore" app-command="maximize" title="Restore"></button>
  <!-- ... -->
</div>
```

### JavaScript Interface

#### Using the `hostWindow` Object

WinFormedge provides the `hostWindow` object, through which you can call window interaction-related methods. Here are some examples of commonly used window control methods:

```javascript
// Close window
document.getElementById("close-button").addEventListener("click", () => {
  hostWindow.close();
});

// Minimize window
document.getElementById("minimize-button").addEventListener("click", () => {
  hostWindow.minimize();
});

// Maximize or restore window
document.getElementById("maximize-button").addEventListener("click", () => {
  hostWindow.windowState === "maximized"
    ? hostWindow.restore()
    : hostWindow.maximize();
});

// Toggle fullscreen state
document.getElementById("fullscreen-button").addEventListener("click", () => {
  hostWindow.toggleFullscreen();
});
```

The `hostWindow` object also provides some properties and methods. You can refer to the following list:
| Method/Property |Type| Description |
|------------------|----|-----------------------------------------|
|activated|`bool`|Indicates whether the window is in an activated state.|
|windowState|`string`|Gets the current window state. Possible values include:<br/> `normal` - Normal state.<br/> `minimized` - Minimized state.<br/> `maximized` - Maximized state.<br/> `fullscreen` - Fullscreen state.|
|hasTitleBar|`bool`|Indicates whether the window displays a title bar.|
|left|`number`|Gets or sets the left position of the window (in pixels).|
|top|`number`|Gets or sets the top position of the window (in pixels).|
|width|`number`|Gets or sets the width of the window (in pixels).|
|height|`number`|Gets or sets the height of the window (in pixels).|
|activate()|`function`|Activates the window, making it the foreground window.|
|minimize()|`function`|Minimizes the window to the taskbar.|
|maximize()|`function`|Maximizes the window.|
|restore()|`function`|Restores the window to its normal size and position.|
|fullscreen()|`function`|Switches the window to fullscreen mode (requires `AllowFullscreen` property set to `true` in the form for this method to work).|
|toggleFullscreen()|`function`|Toggles the window's fullscreen state (requires `AllowFullscreen` property set to `true` in the form for this method to work).|
|close()|`function`|Closes the window.|

#### Window Events

`WinFormedge` also provides some events for windows. You can listen to these events to respond to changes in window state. Below are some commonly used window events. If an event includes parameters, you can get these parameters from the `detail` object of the event's `event` parameter.

| Event Name        | Description                                                             | Parameters                                                                                                                                                                                                                        |
| ----------------- | ----------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| windowactivated   | Triggered when the window is activated.                                 | None                                                                                                                                                                                                                              |
| windowdeactivate  | Triggered when the window loses focus.                                  | None                                                                                                                                                                                                                              |
| windowresize      | Triggered when the window size changes.                                 | `{x:int, y:int, width:int, height:int}` - Object containing the window's new position and size information.                                                                                                                       |
| windowmove        | Triggered when the window position changes.                             | `{x:int, y:int, screenX:int, screenY:int}` - Object containing the window's new position and screen position.                                                                                                                     |
| windowstatechange | Triggered when window state (maximized, minimized, fullscreen) changes. | `{state:string}` - Object containing the new window state. Values of `state` include:<br/> `normal` - Normal state.<br/> `minimized` - Minimized state.<br/> `maximized` - Maximized state.<br/> `fullscreen` - Fullscreen state. |

Here's an example showing how to listen to window events:

```javascript
// Listen to window resize events
window.addEventListener("windowresize", (e) => {
  const { width, height } = e.detail;
  console.log(`Window size changed to: ${width}x${height}`);
});
window.addEventListener("windowmove", (e) => {
  const { x, y } = e.detail;
  console.log(`Window position changed to: X:${x} Y:${y}`);
});
```

## Related Contents

- [Window Styles Configuration](./window-styles.md)
- [Using Different Window Styles](./using-different-window-styles.md)
