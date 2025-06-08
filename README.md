# InterCargo AI Assistant Documentation

This repository contains custom instructions and documentation to help AI assistants effectively work with the InterCargo codebase. These resources are designed to provide AI with the necessary context, patterns, and domain knowledge to assist developers working on the InterCargo shipping container quotation management system.

## Contents

This documentation consists of three main files:

1. **[InterCargo_AI_Instructions.md](InterCargo_AI_Instructions.md)** - Comprehensive overview of the project structure, architecture, and functionality
2. **[InterCargo_AI_Prompting_Guide.md](InterCargo_AI_Prompting_Guide.md)** - Strategies for effectively prompting AI assistants with example prompts and scenarios
3. **[InterCargo_Code_Patterns.md](InterCargo_Code_Patterns.md)** - Detailed examples of code patterns used throughout the codebase

## How to Use These Resources

### For Developers

When working with AI assistants on the InterCargo codebase:

1. **Share the Documentation**: Provide the AI with these files at the beginning of your conversation to establish context.

2. **Follow the Prompting Guide**: Structure your questions and requests according to the patterns in the prompting guide for more effective assistance.

3. **Reference Code Patterns**: When asking for code generation or modifications, refer to the code patterns document to ensure consistency with the existing codebase.

4. **Provide Specific Context**: Always include file paths, relevant code snippets, and your specific goals when asking for help.

### Example Usage

Here's an example of how to effectively prompt an AI assistant after sharing these documents:

```
I'm working on adding a new feature to the InterCargo system that allows customers to specify delivery time preferences in their quotation requests. 

I need to modify the Quotation entity to include this new field and update the relevant UI components and business logic.

Here's the current Quotation entity:
[paste relevant code from InterCargo/BusinessLogic/Entities/Quotation.cs]

Can you help me implement this feature following the existing architectural patterns?
```

## Key Benefits

- **Consistency**: Ensures AI-generated code follows the established patterns and conventions of the InterCargo codebase
- **Efficiency**: Reduces the need to repeatedly explain the project structure and architecture
- **Quality**: Improves the quality of AI assistance by providing detailed domain knowledge and context
- **Learning**: Helps new developers understand the codebase more quickly through comprehensive documentation

## Maintenance

These documentation files should be updated whenever significant changes are made to the InterCargo system architecture, patterns, or domain model. This ensures that AI assistants always have the most current information about the codebase.

## Project Overview

InterCargo is a cargo management system built with ASP.NET Core that facilitates quotation management for shipping containers. The application serves two main user types:

1. **Customers/Users**: Can request quotations for shipping containers and respond to quotations
2. **Employees**: Can review, prepare, and manage quotations submitted by customers

For a complete overview of the system, refer to the [InterCargo_AI_Instructions.md](InterCargo_AI_Instructions.md) file.
