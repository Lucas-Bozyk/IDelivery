# IDelivery API (.NET 9)

Backend para delivery de alimentos em ASP.NET Core Web API com foco em DDD + arquitetura hexagonal.

## Stack

- .NET 9 / C#
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- JWT (Auth + Roles)
- Swagger/OpenAPI
- AutoMapper
- xUnit (integração + regras de domínio)

## Estrutura

Projeto principal:

```
IDelivery/
  API/
  Application/
    DTOs/
    IServices/
    Services/
    UseCases/
    Mappings/
  Domain/
    Entities/
    ValueObjects/
    Interfaces/IRepositories/
  Infrastructure/
    Config/
    Security/
  Persistence/
    Repositories/
    IdentityDbContext.cs
    DeliveryDbContext.cs
```

Projeto de testes:

```
IDelivery.Tests/
```

## Funcionalidades principais

- Cadastro/login/refresh token com JWT
- Controle de roles: `Admin`, `Customer`, `RestaurantOwner`, `DeliveryDriver`
- Fluxos de restaurante, produto, carrinho, pedido, pagamento, entrega, cupom e review
- Regras de negócio centrais concentradas no domínio (entidades)

## Regras de negócio implementadas

- Produto indisponível não entra no carrinho
- Carrinho/pedido com produtos de um único restaurante
- Restaurante fechado não recebe pedido
- Pedido calcula subtotal/taxa/desconto/total
- Cupom valida expiração, mínimo e limite de uso
- Pagamento aprovado confirma pedido
- Pedido cancelado não vai para entrega
- Entrega só conclui com pagamento aprovado
- Review só para pedido concluído

## Configuração de ambiente

Crie seu `.env` a partir de `IDelivery/.env.example`:

```env
Jwt__Issuer=IDelivery
Jwt__Audience=IDelivery.Client
Jwt__Key=YOUR_32_PLUS_CHAR_SECRET

ADMIN_EMAIL=admin@idelivery.local
ADMIN_PASSWORD=CHANGE_ME_STRONG
```

Também há slots opcionais para seed de:
- customer
- restaurant owner
- delivery driver

## Seed

No startup, a API:
- cria bancos (`EnsureCreated`)
- garante roles
- cria usuários de seed por variáveis de ambiente (se informadas)

Local do seed: `IDelivery/Program.cs`.

## Rodar localmente

```bash
dotnet restore IDelivery/IDelivery.csproj --configfile NuGet.Config
dotnet run --project IDelivery/IDelivery.csproj
```

Swagger:
- `http://localhost:<porta>/swagger`

## Testes

```bash
dotnet test IDelivery.Tests/IDelivery.Tests.csproj --no-restore
```

## Segurança (mínimo)

- Não commitar `.env`
- Não usar credenciais default em produção
- Rotacionar `Jwt__Key`
- Ativar HTTPS/reverse proxy e observabilidade
- Evoluir para migrations versionadas e política de secrets (Vault/KeyVault/Secrets Manager)

## Situação para Git

Pronto para versionar como base de desenvolvimento, com:
- build passando
- testes passando
- `.gitignore` e `.env.example` adicionados

Antes de produção, evoluir:
- migrations EF oficiais
- validação formal de entrada (FluentValidation)
- políticas de autorização por ownership (resource-based)
