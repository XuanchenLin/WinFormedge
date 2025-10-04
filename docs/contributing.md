# 贡献指南

欢迎为 WinFormedge 项目做出贡献！本指南将帮助您了解如何参与项目开发。

## 目录

- [开发环境设置](#开发环境设置)
- [贡献方式](#贡献方式)
- [代码规范](#代码规范)
- [提交流程](#提交流程)
- [问题报告](#问题报告)
- [功能请求](#功能请求)

---

## 开发环境设置

### 系统要求

- **操作系统**: Windows 10 版本 1903 或更高版本
- **IDE**: Visual Studio 2022 (推荐) 或 Visual Studio Code
- **.NET**: .NET 8.0 SDK 或更高版本
- **Git**: 用于版本控制

### 环境配置

1. **克隆仓库**：
   ```bash
   git clone https://github.com/XuanchenLin/WinFormedge.git
   cd WinFormedge
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **构建项目**：
   ```bash
   dotnet build
   ```

4. **运行测试**：
   ```bash
   dotnet test
   ```

### 开发工具推荐

- **Visual Studio 2022**: 主要开发 IDE
- **VS Code**: 轻量级代码编辑
- **Git Extensions**: Git 图形化工具
- **NuGet Package Explorer**: NuGet 包管理工具
- **ILSpy**: .NET 反编译工具

---

## 贡献方式

### 代码贡献

1. **Fork 项目**: 在 GitHub 上 fork 主仓库
2. **创建分支**: 为您的功能或修复创建新分支
3. **开发代码**: 实现您的功能或修复
4. **编写测试**: 确保代码有适当的测试覆盖
5. **提交 PR**: 向主仓库提交 Pull Request

### 文档贡献

- 改进现有文档
- 翻译文档到其他语言
- 添加代码示例
- 创建教程和指南

### 社区贡献

- 回答社区问题
- 参与讨论
- 报告和验证问题
- 推广项目

---

## 代码规范

### C# 编码标准

#### 命名约定

```csharp
// 类名使用 PascalCase
public class WindowManager { }

// 方法名使用 PascalCase
public void CreateWindow() { }

// 属性使用 PascalCase
public string WindowTitle { get; set; }

// 私有字段使用 _camelCase
private readonly string _windowTitle;

// 常量使用 UPPER_CASE
private const string DEFAULT_WINDOW_TITLE = "Default";

// 参数和局部变量使用 camelCase
public void SetTitle(string newTitle)
{
    var formattedTitle = FormatTitle(newTitle);
}
```

#### 代码组织

```csharp
// 文件头注释
// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Web.WebView2.Core;
using WinFormedge.WebResource;

namespace WinFormedge.HostForms;

/// <summary>
/// 窗口管理器类，负责创建和管理应用程序窗口。
/// </summary>
public class WindowManager
{
    #region Fields
    
    private readonly List<Formedge> _windows = new();
    private Formedge? _mainWindow;
    
    #endregion
    
    #region Properties
    
    /// <summary>
    /// 获取当前活动窗口数量。
    /// </summary>
    public int ActiveWindowCount => _windows.Count;
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// 初始化窗口管理器的新实例。
    /// </summary>
    public WindowManager()
    {
        // 初始化代码
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 创建新窗口。
    /// </summary>
    /// <param name="settings">窗口设置</param>
    /// <returns>创建的窗口实例</returns>
    public Formedge CreateWindow(WindowSettings settings)
    {
        // 实现
    }
    
    #endregion
    
    #region Private Methods
    
    private void RegisterWindow(Formedge window)
    {
        // 实现
    }
    
    #endregion
}
```

#### XML 文档注释

```csharp
/// <summary>
/// 表示 WinFormedge 窗口的基类。
/// </summary>
/// <remarks>
/// 此类提供了 WebView2 和 Windows Forms 的集成功能。
/// </remarks>
/// <example>
/// <code>
/// public class MyWindow : Formedge
/// {
///     public MyWindow()
///     {
///         Url = "https://example.com";
///         Size = new Size(800, 600);
///     }
/// }
/// </code>
/// </example>
public abstract class Formedge : FormBase
{
    /// <summary>
    /// 获取或设置窗口的初始 URL。
    /// </summary>
    /// <value>
    /// 表示要在 WebView2 中加载的 URL 的字符串。
    /// </value>
    /// <exception cref="ArgumentException">
    /// 当 URL 格式无效时抛出。
    /// </exception>
    public string? Url { get; set; }
}
```

### JavaScript/TypeScript 规范

```javascript
// 使用 const/let 而不是 var
const API_BASE_URL = 'https://api.local';
let currentUser = null;

// 函数命名使用 camelCase
function calculateWindowSize() {
    // 实现
}

// 类命名使用 PascalCase
class WindowController {
    constructor() {
        this.isInitialized = false;
    }
    
    async initialize() {
        // 异步初始化
    }
}

// 使用箭头函数
const handleClick = (event) => {
    event.preventDefault();
    // 处理点击事件
};

// JSDoc 注释
/**
 * 创建新的窗口实例
 * @param {Object} options - 窗口配置选项
 * @param {string} options.title - 窗口标题
 * @param {number} options.width - 窗口宽度
 * @param {number} options.height - 窗口高度
 * @returns {Promise<boolean>} 创建是否成功
 */
async function createWindow(options) {
    // 实现
}
```

### CSS 规范

```css
/* 使用 BEM 命名约定 */
.window {
    /* 块级元素 */
}

.window__header {
    /* 元素 */
}

.window__header--minimized {
    /* 修饰符 */
}

/* CSS 自定义属性 */
:root {
    --primary-color: #0078d4;
    --secondary-color: #106ebe;
    --text-color: #323130;
    --bg-color: #ffffff;
}

/* 媒体查询 */
@media (prefers-color-scheme: dark) {
    :root {
        --text-color: #ffffff;
        --bg-color: #1f1f1f;
    }
}
```

---

## 提交流程

### Git 工作流

1. **创建功能分支**：
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **进行开发**：实现您的功能或修复

3. **提交更改**：
   ```bash
   git add .
   git commit -m "feat: add new window management feature"
   ```

4. **推送分支**：
   ```bash
   git push origin feature/your-feature-name
   ```

5. **创建 Pull Request**：在 GitHub 上创建 PR

### 提交信息规范

使用 [Conventional Commits](https://www.conventionalcommits.org/) 规范：

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**类型说明**：
- `feat`: 新功能
- `fix`: 错误修复
- `docs`: 文档更新
- `style`: 代码格式更改
- `refactor`: 重构代码
- `test`: 添加或修改测试
- `chore`: 构建或工具更改

**示例**：
```
feat(window): add support for custom title bar

- Implement custom title bar rendering
- Add drag region support
- Update window settings API

Closes #123
```

### Pull Request 模板

创建 PR 时，请包含以下信息：

```markdown
## 变更描述
简要描述您的更改。

## 变更类型
- [ ] 错误修复
- [ ] 新功能
- [ ] 破坏性变更
- [ ] 文档更新

## 测试
描述您如何测试这些更改。

## 清单
- [ ] 代码遵循项目的代码规范
- [ ] 自我审查了代码
- [ ] 添加了必要的注释
- [ ] 添加了相应的测试
- [ ] 所有测试都通过
- [ ] 更新了相关文档
```

---

## 问题报告

### 问题模板

报告问题时，请提供以下信息：

```markdown
## 问题描述
清楚简洁地描述问题。

## 重现步骤
1. 打开应用程序
2. 点击 '...'
3. 看到错误

## 预期行为
描述您期望发生的情况。

## 实际行为
描述实际发生的情况。

## 环境信息
- OS: Windows 11
- .NET Version: .NET 8.0
- WinFormedge Version: 1.0.0
- WebView2 Version: 120.0.2210.91

## 附加信息
添加任何其他有助于解决问题的信息。
```

### 问题标签

使用适当的标签分类问题：

- `bug`: 错误报告
- `enhancement`: 功能增强
- `documentation`: 文档相关
- `good first issue`: 适合新贡献者
- `help wanted`: 需要帮助
- `question`: 问题咨询

---

## 功能请求

### 请求模板

```markdown
## 功能描述
简洁清晰地描述您想要的功能。

## 问题解决
这个功能解决了什么问题？

## 建议的解决方案
描述您希望如何实现这个功能。

## 替代方案
描述您考虑过的任何替代解决方案或功能。

## 附加上下文
添加任何其他有关功能请求的上下文或截图。
```

### 功能评估标准

我们根据以下标准评估功能请求：

1. **与项目目标的一致性**
2. **用户需求和影响范围**
3. **实现复杂度**
4. **维护成本**
5. **向后兼容性**

---

## 社区准则

### 行为准则

- 尊重所有贡献者
- 提供建设性的反馈
- 专注于最适合社区的事情
- 对新贡献者表示同理心

### 沟通指南

- 使用清晰、礼貌的语言
- 提供具体的、可操作的反馈
- 承认他人的贡献
- 保持专业态度

---

## 发布流程

### 版本控制

使用 [Semantic Versioning](https://semver.org/)：

- **MAJOR**: 不兼容的 API 更改
- **MINOR**: 向后兼容的功能添加
- **PATCH**: 向后兼容的错误修复

### 发布清单

1. 更新版本号
2. 更新 CHANGELOG.md
3. 创建发布标签
4. 构建和测试包
5. 发布到 NuGet
6. 创建 GitHub Release

---

## 资源链接

- **GitHub 仓库**: [https://github.com/XuanchenLin/WinFormedge](https://github.com/XuanchenLin/WinFormedge)
- **问题跟踪**: [https://github.com/XuanchenLin/WinFormedge/issues](https://github.com/XuanchenLin/WinFormedge/issues)
- **讨论区**: [https://github.com/XuanchenLin/WinFormedge/discussions](https://github.com/XuanchenLin/WinFormedge/discussions)
- **Wiki**: [https://github.com/XuanchenLin/WinFormedge/wiki](https://github.com/XuanchenLin/WinFormedge/wiki)

感谢您对 WinFormedge 项目的贡献！