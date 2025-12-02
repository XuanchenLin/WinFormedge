# 窗体交互 API

在 WinFormedge 中，框架提供了一系列窗体交互 API，帮助您实现自定义标题栏按钮、拖动窗口、调整窗口大小等功能。这些 API 有些基于 HTML 和 CSS 属性，有些基于 WinFormedge 提供的 JavaScript 接口。您可以参考以下内容，了解如何使用这些 API 来增强您的应用程序的用户体验。

## 移动窗口

要实现拖动窗口的功能，您可以使用 CSS 的 `-webkit-app-region` 属性。将该属性设置为 `drag` 的区域可以用来拖动窗口，而将其设置为 `no-drag` 的区域则不会响应拖动操作。

```css
.title-bar {
  -webkit-app-region: drag; /* 允许拖动窗口 */
}
.title-bar .close-button {
  -webkit-app-region: no-drag; /* 不允许拖动窗口 */
}
```

在上面的示例中，`.title-bar` 类的元素可以用来拖动窗口，而 `.close-button` 类的元素则不会响应拖动操作。

## 控制窗体行为

您可以使用 WinFormedge 提供的 html 属性和 JavaScript 接口来实现自定义标题栏按钮的功能。例如，您可以创建自定义的关闭、最小化和最大化按钮，并绑定相应的事件处理程序。

WinFormege 提供了**关闭窗口**、**最小化窗口**、**最大化/还原窗口**和**窗口全屏化**的接口，您可以使用 html 属性或 JavaScript 来调用这些接口。

### HTML 属性

您也可以直接在 HTML 元素中使用 `app-command` 属性来绑定窗口控制命令。以下是一个示例，展示了如何创建自定义的标题栏按钮：

```html
<div>
  <button class="fullscreen" app-command="fullscreen" title="全屏"></button>
  <button class="minimize" app-command="minimize" title="最小化"></button>
  <button class="maximize" app-command="maximize" title="最大化"></button>
  <button class="close" app-command="close" title="关闭"></button>
</div>
```

此外，`WinFormege` 还提为当前窗体状态提供了一些 CSS 类名以指示窗体当前的各种状态，这些类名被自动添加到 `html` 元素上，您可以根据这些类名来调整前端页面在不同窗体状态下的显示效果：

| 类名                       | 描述               |
| -------------------------- | ------------------ |
| window--activated          | 窗体处于激活状态   |
| window--deactivated        | 窗体处于非激活状态 |
| window--maximized          | 窗体处于最大化状态 |
| window--minimized          | 窗体处于最小化状态 |
| window--fullscreen         | 窗体处于全屏状态   |
| window\_\_titlebar--shown  | 标题栏显示         |
| window\_\_titlebar--hidden | 标题栏隐藏         |

下面的示例将展示如何利用 `window--maximized` 类名来控制最大化按钮和还原按钮的显示与隐藏：

CSS 样式：

```css
.restore {
  display: none; /* 默认隐藏还原按钮 */
}
.window--maximized .maximize {
  display: none; /* 窗体最大化时隐藏最大化按钮 */
}
.window--maximized .restore {
  display: block; /* 窗体最大化时显示还原按钮 */
}
```

HTML 代码：

```html
<div>
  <!-- ... -->
  <button class="maximize" app-command="maximize" title="最大化"></button>
  <button class="restore" app-command="maximize" title="还原"></button>
  <!-- ... -->
</div>
```

### JavaScript 接口

#### 使用 `hostWindow` 对象

WinFormedge 提供了 `hostWindow` 对象，您可以通过该对象调用窗体交互相关的方法。以下是一些常用的窗口控制方法示例：

```javascript
// 关闭窗口
document.getElementById("close-button").addEventListener("click", () => {
  hostWindow.close();
});

// 最小化窗口
document.getElementById("minimize-button").addEventListener("click", () => {
  hostWindow.minimize();
});

// 最大化或还原窗口
document.getElementById("maximize-button").addEventListener("click", () => {
  hostWindow.windowState === "maximized"
    ? hostWindow.restore()
    : hostWindow.maximize();
});

// 切换全屏状态
document.getElementById("fullscreen-button").addEventListener("click", () => {
  hostWindow.toggleFullscreen();
});
```

`hostWindow` 对象还提供了一些属性和方法，您可以参考以下列表：
| 方法/属性 |类型| 描述 |
|------------------|----|-----------------------------------------|
|activated|`bool`|指示窗口是否处于激活状态。|
|windowState|`string`|获取当前窗口状态，可能的值包括：<br/> `normal` - 普通状态。<br/> `minimized` - 最小化状态。<br/> `maximized` - 最大化状态。<br/> `fullscreen` - 全屏状态。|
|hasTitleBar|`bool`|指示窗口是否显示标题栏。|
|left|`number`|获取或设置窗口的左侧位置（以像素为单位）。|
|top|`number`|获取或设置窗口的顶部位置（以像素为单位）。|
|width|`number`|获取或设置窗口的宽度（以像素为单位）。|
|height|`number`|获取或设置窗口的高度（以像素为单位）。|
|activate()|`function`|激活窗口，使其成为前台窗口。|
|minimize()|`function`|将窗口最小化到任务栏。|
|maximize()|`function`|将窗口最大化。|
|restore()|`function`|将窗口还原到普通大小和位置。|
|fullscreen()|`function`|将窗口切换到全屏模式（需要在窗体中将 `AllowFullscreen` 属性设置为 `true` 此方法方可生效）。|
|toggleFullscreen()|`function`|切换窗口的全屏状态。（需要在窗体中将 `AllowFullscreen` 属性设置为 `true` 此方法方可生效）|
|close()|`function`|关闭窗口。|

#### 窗口事件

`WinFormedge` 还为窗口提供了一些事件，您可以监听这些事件以响应窗口状态的变化。以下是一些常用的窗口事件，如果事件包括参数，那么您可以从事件的 `event` 参数的 `detail` 对象中获取这些参数。

| 事件名称          | 描述                                                 | 参数                                                                                                                                                                                  |
| ----------------- | ---------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| windowactivated   | 当窗口被激活时触发。                                 | 无                                                                                                                                                                                    |
| windowdeactivate  | 当窗口失去焦点时触发。                               | 无                                                                                                                                                                                    |
| windowresize      | 当窗口大小发生变化时触发。                           | `{x:int, y:int, width:int, height:int}` - 包含窗口的新位置和大小信息的对象。                                                                                                          |
| windowmove        | 当窗口位置发生变化时触发。                           | `{x:int, y:int, screenX:int, screenY:int}` - 包含窗口的新位置和屏幕位置的对象。                                                                                                       |
| windowstatechange | 当窗口状态（如最大化、最小化、全屏）发生变化时触发。 | `{state:string}` - 包含新窗口状态的对象。`state` 的值包括：<br/> `normal` - 正常状态。<br/> `minimized` - 最小化状态。<br/> `maximized` - 最大化状态。<br/> `fullscreen` - 全屏状态。 |

以下是一个示例，展示了如何监听窗口事件：

```javascript
// 监听窗口的尺寸变化事件
window.addEventListener("windowresize", (e) => {
  const { width, height } = e.detail;
  console.log(`窗口尺寸变化为: ${width}x${height}`);
});
window.addEventListener("windowmove", (e) => {
  const { x, y } = e.detail;
  console.log(`窗口位置变化为: X:${x} Y:${y}`);
});
```

## 相关内容

- [窗口样式配置](./window-styles.zh-CN.md)
- [使用不同的窗口样式](./using-different-window-styles.zh-CN.md)
