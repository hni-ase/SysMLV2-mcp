# csharp-mcp-example

This repository contains a C# Model Context Protocol (MCP) server for working with SysML v2 models and related services.

✅ **Research note**: The paper describing this framework has been accepted to the **DESIGN 2026** conference.

The bibtex entry to cite this paper is coming soon. b

📄 **License**: This repository is licensed under the **MIT License**.

## Run the MCP server (VS Code)

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

### Test the MCP server in VS Code

You can use the built-in REST client via the `.http` file included in this repo.

1. Open `mcp/csharp-mcp-example.http` in VS Code.
2. Start the server if it isn’t running.
3. Click **Send Request** above any request definition to issue it.

> Tip: If you don’t see the **Send Request** links, install the **REST Client** extension in VS Code.

## Run tests

From VS Code’s integrated terminal:

```bash
dotnet test
```

## Development commands

Regenerate the SysML v2 client:

```bash
openapi-generator-cli generate -i sysmlv2-api-spec/openapi_client.yml -g csharp -o sysml-v2-client
```

Regenerate the SysML model classes from Ecore:

```bash
dotnet Ecore2Code -f -l CS -n mcp.Src.Models -o mcp/Src/Models/SysML SysML.ecore --no-events -x --primitive-types --no-changed --no-changing --collectionsAreElements
```