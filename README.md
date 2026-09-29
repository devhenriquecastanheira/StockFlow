# StockFlow

StockFlow é uma aplicação web para gerenciamento de estoque, produtos, fornecedores, clientes, compras e pedidos.

## Sobre o projeto

O projeto foi desenvolvido com ASP.NET Core e organizado em camadas, separando interface web, API, regras de negócio, domínio e acesso a dados.

## Tecnologias

- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- FluentValidation
- QuestPDF
- MailKit
- Bootstrap

## Funcionalidades

- Cadastro de produtos, variantes, categorias e tags
- Gestão de fornecedores e armazéns
- Controle de estoque por armazém
- Registro de movimentações e transferências de estoque
- Gestão de ordens de compra
- Carrinho, checkout e pedidos
- Cadastro de clientes e endereços
- Geração de invoices
- Envio de e-mail após checkout

## Estrutura do projeto

```text
StockFlow.Api             API da aplicação
StockFlow.Web             Interface web MVC
StockFlow.Application     Serviços e regras de negócio
StockFlow.Domain          Entidades, enums e contratos
StockFlow.Infrastructure  Persistência e repositórios
```

## Como executar

Restaure as dependências:

```powershell
dotnet restore
```

Compile a solução:

```powershell
dotnet build
```

Execute a API:

```powershell
dotnet run --project StockFlow.Api
```

Em outro terminal, execute a interface web:

```powershell
dotnet run --project StockFlow.Web
```

## Configuração

A API utiliza PostgreSQL e autenticação com JWT. Configure a string de conexão, a chave JWT e as credenciais de e-mail no ambiente local antes de executar o projeto.
