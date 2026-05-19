# csharp-mcp-example

This repository contains a C# Model Context Protocol (MCP) server for working with SysML v2 models and related services.

✅ **Research note**: The paper describing this framework has been accepted to the **DESIGN 2026** conference.

The bibtex entry to cite this paper is coming soon. b

📄 **License**: This repository is licensed under the **MIT License**.

## MCP Tools

All tools are defined in [mcp/Src/Tools/ModelCreationTool.cs](mcp/Src/Tools/ModelCreationTool.cs) as static methods annotated with `[McpServerTool]`.

### Project management

| Tool | Description |
|---|---|
| `CreateProject` | Creates a new SysML V2 project. |
| `GetProjects` | Lists all projects from the SysML v2 API. |
| `GetProjectByName` | Looks up a project by name. |

### Element querying

| Tool | Description |
|---|---|
| `GetElementsFromProjectHead` | Returns elements from the head commit of a project's default branch. Supports optional `elementType` and `nameContains` filters. |
| `GetAllElementsFromProjectHead` | Convenience wrapper — returns all elements with no filters applied. |
| `GetElementsByTypeFromProjectHead` | Returns elements filtered by a specific SysML v2 type name. |
| `GetPackagesFromProjectHead` | Returns only `Package` and `LibraryPackage` elements, with an optional name filter. |
| `GetElementByIdFromProjectHead` | Fetches a single element by GUID and returns its ownership chain. |

### Schema introspection

| Tool | Description |
|---|---|
| `DescribeTypeSchema` | Returns the required and optional attributes for any named SysML v2 element type. |
| `DescribeElementSchema` | Fetches a live element by ID and returns the same schema information scoped to that element's actual type. |

### Element creation

| Tool | Description |
|---|---|
| `CreatePackage` | Creates a package, optionally nested inside a parent package. |
| `CreateTopLevelPackage` | Convenience wrapper for `CreatePackage` at the project root. |
| `CreateRequirement` | Creates a `RequirementUsage` element, optionally nested under a parent package. |
| `CreateRequirementDefinition` | Creates a `RequirementDefinition` element with optional `isAbstract` flag. |
| `CreateUseCase` | Creates a `UseCaseUsage` element, optionally linked to an objective requirement. |
| `CreateElementOfType` | Generic tool — creates any SysML v2 element type given a JSON attributes payload. Invalid schema attributes are reported and skipped rather than causing an error. |

### Element modification

| Tool | Description |
|---|---|
| `UpdateElementAttributes` | Overwrites specific attributes on an existing element while preserving all others. Accepts a JSON object string for the attribute patch. |
| `AddSubjectToRequirement` | Adds a `SubjectMembership` + `ReferenceUsage` (subject parameter) to an existing requirement. |
| `SetRequirementDefinition` | Types a `RequirementUsage` against a `RequirementDefinition` by setting the `requirementDefinition` reference field. |

---

## Contributing new tools

### 1. Understand the project structure

```
mcp/Src/
├── Tools/
│   └── ModelCreationTool.cs      ← all MCP tool methods live here
├── Services/
│   ├── ISysMLApiService.cs       ← API client interface
│   ├── SysMLApiService.cs        ← HTTP implementation against localhost:9000
│   └── FactoryServices/          ← domain factories (package, requirement, use-case …)
└── Models/                       ← return types used by tools
```

### 2. Add a tool method

Open [mcp/Src/Tools/ModelCreationTool.cs](mcp/Src/Tools/ModelCreationTool.cs) and add a `public static` method to the `ModelCreationTools` class:

```csharp
[McpServerTool, Description("One-sentence description shown to the LLM.")]
public static MyReturnType MyNewTool(McpServer server, string projectName, /* … */)
{
    var apiService = RequireApiService(server);
    // implementation
}
```

Key rules:
- The `[McpServerTool]` and `[Description("…")]` attributes are mandatory — the description is what the LLM uses to decide when to call the tool.
- Accept `McpServer server` as the first parameter whenever you need to call the SysML API or the metamodel factory. Retrieve services with the existing `RequireApiService(server)` / `RequireMetaModelFactory(server)` helpers.
- Return a plain C# object, a primitive, or a `List<T>`. The MCP framework serialises return values to JSON automatically.
- Use `.GetAwaiter().GetResult()` to resolve async API calls (the tool methods must be synchronous from the MCP framework's perspective).

### 3. Add a factory (if needed)

For non-trivial creation logic, add a dedicated factory class under `mcp/Src/Services/FactoryServices/` following the pattern of `SysMLRequirementFactory` or `SysMLPackageFactory`:

1. Create `MySysMLXyzFactory.cs` with a constructor that accepts `ISysMLApiService`.
2. Implement your logic, building `CommitRequest` / `DataVersionRequest` payloads and calling `apiService.CommitToBranchAsync(…)`.
3. Call your factory from the tool method in `ModelCreationTool.cs`.

### 4. Define a return type (if needed)

Add a new record or class to `mcp/Src/Models/` for any structured data the tool returns. Keep return types flat and JSON-serialisable so the LLM can interpret them easily.

### 5. Write tests

Add a test class to `csharp-mcp-example-test/` following the existing `SysMLOpenApiFactoryTests.cs` pattern. Run the suite with:

```bash
dotnet test
```

These steps use the built-in VS Code C#/.NET tooling to run and test the MCP server.

### Prerequisites

- .NET SDK (compatible with the project’s target framework)
- VS Code with the **C#** extension installed

### Run the server

1. Open the repository folder in VS Code.
2. In the Explorer, open `mcp/Src/Program.cs`.
3. Press **F5** to run with the debugger, or use **Run > Start Debugging**.
	- If prompted to select an environment, choose the default .NET configuration.
4. Confirm the server is running in the VS Code Debug Console/Terminal output.

