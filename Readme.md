# POC: Conexão com Bancos de Dados usando Entra ID / Azure AD (Active Directory)

## Visão Geral

Este projeto é uma Prova de Conceito (PoC) que demonstra como estabelecer conexões seguras com bancos de dados SQL Server e PostgreSQL utilizando autenticação do Entra ID (anteriormente conhecido como Azure AD) em uma aplicação C#.

## Objetivo

O principal objetivo desta PoC é ilustrar:

1. A integração do Entra ID com bancos de dados SQL Server e PostgreSQL.
2. A implementação de conexões seguras tanto em ambientes locais quanto na nuvem Azure.
3. A utilização de tokens de acesso para autenticação em bancos de dados.


## Funcionalidades Chave

- Autenticação com Entra ID para acesso a bancos de dados
- Conexão com SQL Server usando credenciais do Entra ID
- Conexão com PostgreSQL usando tokens de acesso do Entra ID
- Suporte para ambientes locais e na nuvem Azure

Esta PoC serve como um ponto de partida para desenvolvedores que desejam implementar autenticação segura em aplicações que necessitam acessar bancos de dados SQL Server e PostgreSQL, aproveitando as capacidades de segurança e gerenciamento de identidade do Entra ID da Microsoft.

## Referências 


1. **Conexão com PostgreSQL usando Entra ID no .NET**
   - [Azure PostgreSQL e Entra ID com .NET](https://www.aaron-powell.com/posts/2024-06-03-azure-postgresql-and-entra-id-dotnet/)

3. **Configuração de Autenticação Entra ID para Azure SQL**
   - [Criar Principais do Microsoft Entra no SQL](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure?view=azuresql&tabs=azure-portal#create-microsoft-entra-principals-in-sql)

2. **Documentação Microsoft sobre Autenticação com Entra ID**
   - [Conexão com Identidade Gerenciada (PostgreSQL)](https://learn.microsoft.com/en-us/previous-versions/azure/postgresql/single-server/how-to-connect-with-managed-identity)
   - [Configuração de Login com Autenticação Azure AD (PostgreSQL Flexible Server)](https://learn.microsoft.com/pt-br/azure/postgresql/flexible-server/how-to-configure-sign-in-azure-ad-authentication)
   - [Conceitos de Autenticação Azure AD (PostgreSQL Single Server)](https://learn.microsoft.com/en-us/previous-versions/azure/postgresql/single-server/concepts-azure-ad-authentication)


## SQL Server

Como criar e remover usuários no SQL Server usando o Entra ID:

```sql
---------------- CRIAR USUÁRIO ----------------

-- Conectar ao banco de dados master

USE [master];
GO

-- Criar login para o usuário do Azure AD
CREATE LOGIN [UserEmailOuGroupName] FROM EXTERNAL PROVIDER;
GO

-- Conectar ao banco de dados onde você quer conceder permissões
USE [poc-ad-db];
GO

-- Criar usuário no banco de dados
CREATE USER [UserEmailOuGroupName] FROM EXTERNAL PROVIDER;
GO

-- Adicionar usuário ao papel db_datareader para permissões de leitura
ALTER ROLE db_datareader ADD MEMBER [UserEmailOuGroupName];
GO

-- Adicionar usuário ao papel db_datawriter para permissões de escrita
ALTER ROLE db_datawriter ADD MEMBER [UserEmailOuGroupName];
GO

-- APENAS PARA APIS e users que precisam ALTERAR O BANCO
ALTER ROLE db_ddladmin ADD MEMBER [UserEmailOuGroupName];
GO

---------------- DELETAR USUÁRIO ----------------

-- Conectar ao banco de dados específico
USE [poc-ad-db];
GO

-- Remover o usuário das funções (roles) do banco de dados
ALTER ROLE db_datareader DROP MEMBER [UserEmailOuGroupName];
GO

ALTER ROLE db_datawriter DROP MEMBER [UserEmailOuGroupName];
GO

-- Deletar o usuário do banco de dados
DROP USER [UserEmailOuGroupName];
GO

-- Conectar ao banco de dados master
USE master;
GO

-- Deletar o login do servidor
DROP LOGIN [UserEmailOuGroupName];
GO
```


## PostgreSQL

Como criar e remover usuários no PostgreSQL usando o Entra ID:

```sql
-- TODO: Criar script
```
