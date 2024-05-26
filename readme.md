# FluentDebugger

FluentDebugger 是一個方便的工具，用於在執行同步和異步方法時記錄執行時間和方法參數，並提供一個可自訂的例外處理機制。

## 功能

- 記錄同步方法的執行時間
- 記錄異步方法的執行時間
- 可選擇記錄方法參數
- 支援自訂的例外處理機制

## 安裝

```shell
dotnet add package FluentDebugger
```

## 使用方法

### 建立 FluentDebugger

您可以使用 `Create` 方法來建立 `FluentDebugger` 實例，可以選擇傳入自訂的 `ILogAdapter` 或是省略 log紀錄。

```csharp
var debugger = FluentDebugger.Create(); // 省略 log
var debuggerWithLogger = FluentDebugger.Create(customLogger); // 使用自訂的 ILogAdapter
```

### 記錄同步方法執行時間

使用 `Run` 方法來執行並記錄同步方法的執行時間。

```csharp
var result = FluentDebugger.Create(customLogger)
    .Run(() => SomeMethod());
// output: Execution time: xxx ms    
```

### 記錄異步方法執行時間

使用 `RunAsync` 方法來執行並記錄異步方法的執行時間。

```csharp
var result = await FluentDebugger.Create(customLogger)
    .RunAsync(() => SomeAsyncMethod());
// output: Execution time: xxx ms    
```

### 記錄方法參數

使用 `LogParameters` 方法來記錄方法的參數。

```csharp
var result = FluentDebugger.Create(customLogger)
    .LogParameters()
    .Run(() => SomeMethodWithParameters(100, new List<int> { 1, 2, 3 }));
// output: "[SomeMethodWithParameters()] Parameters: `param1: 100, param2: [1, 2, 3]` | Execution time: {watch.ElapsedMilliseconds}ms    
```

### 自訂例外處理機制

使用 `Rethrow` 方法來自訂例外處理機制。

```csharp
var result = FluentDebugger.Create(customLogger)
    .LogParameters()
    .Rethrow(exceptionContext => new CustomException(exceptionContext.Exception, $"{exceptionContext.MethodName} 出現錯誤"))
    .Run(() => SomeMethodThatThrows());
```

### ExceptionContext
除了在 **自訂例外處理機制**可以使用 `ExceptionContext` 類別外，在 `ILogAdapter` 中會自動將 `ExceptionContext`包進 message中。
```csharp
public class ExceptionContext
{
    public Exception Exception { get; set; }
    public string MethodName { get; set; }
    public List<KeyValuePair<string, object>> Parameters { get; set; }
    public long ElapsedMilliseconds { get; set; }
}
```
1. **自動解析方法名稱**：自動從表達式中提取方法名稱，無需手動指定，減少錯誤。
2. **參數擷取和格式化**：自動擷取並格式化方法參數，支持字典和集合類型，使記錄更直觀。
3. **支援不同類型的參數**：能夠處理各種參數類型，包括基本類型、集合和字典，提供靈活性。
4. **簡化除錯過程**：結合 `FluentDebugger` 類別，可以輕鬆記錄方法呼叫和參數，有助於快速定位問題。