# LastLink API - Advance Payment Request System

REST API desenvolvida em **.NET 9** para gestão de solicitações de antecipação de recebíveis.  
Implementa **Clean Architecture**, **Domain-Driven Design (DDD)** .
---

## 🏗️ Arquitetura

-----------------------------------------------------------------------------------

Domain → Entidades, Value Objects e regras de negócio.

Application → Commands, Queries e Handlers (CQRS + MediatR).

Infrastructure → Repositórios + EF Core (SQLite em dev, Postgres em produção).

API → Controllers versionadas (v1, v2), Swagger/OpenAPI e autenticação JWT.

-----------------------------------------------------------------------------------
🚀 Como Rodar
📦 Pré-requisitos

.NET 9.0 SDK

Docker e Docker Compose (opcional)

▶️ Execução Local(bash)

# Clone o repositório
git clone https://github.com/afjunior2/lastlink-api.git
cd lastlink-api

# Restore das dependências
dotnet restore

# Rodar aplicação
cd LastLinkApi.Api
dotnet run --urls="http://0.0.0.0:8080"

-----------------------------------------------------------------------------------

🐳 Execução com Docker Compose
# Build da imagem
docker-compose build --no-cache

# Subir em modo dev (SQLite)
docker-compose up -d

# Subir em modo produção (PostgreSQL)
docker-compose --profile production up -d

-----------------------------------------------------------------------------------

🧪 Testes
Executar todos os testes
dotnet test

Com relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/coverage.json /p:CoverletOutputFormat=opencover

Gerar relatório HTML
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:TestResults/coverage.json -targetdir:coveragereport


👉 Abra coveragereport/index.html no navegador.

-----------------------------------------------------------------------------------

📖 Endpoints da API
🔐 Autenticação

--

POST /auth/token?userId={id} → Gera token JWT fake (mock).

🩺 Health Check

--

GET /health → Verifica status da aplicação.

---

📌 Advance Requests v1
--
POST /api/v1/AdvanceRequests → Criar solicitação.
--
GET /api/v1/AdvanceRequests/{id} → Buscar por Id.
--
GET /api/v1/AdvanceRequests/creator/{creatorId} → Buscar por criador.
--
PUT /api/v1/AdvanceRequests/{id}/approve → Aprovar solicitação.
--
PUT /api/v1/AdvanceRequests/{id}/reject → Recusar solicitação.
--
GET /api/v1/AdvanceRequests/simulate?requestedAmount=1000 → Simular solicitação.

---

📌 Advance Requests v2

(Similares à v1, mas retornam também o campo netAmount)

POST /api/v2/AdvanceRequests

GET /api/v2/AdvanceRequests/{id}

GET /api/v2/AdvanceRequests/creator/{creatorId}

PUT /api/v2/AdvanceRequests/{id}/approve

PUT /api/v2/AdvanceRequests/{id}/reject

GET /api/v2/AdvanceRequests/simulate?requestedAmount=1000

-----------------------------------------------------------------------------------

📝 Estrutura dos Campos
📌 AdvanceRequest
id	              int	        Identificador único da solicitação

creatorId	        string	    Identificador do criador/usuário

requestedAmount	  decimal	    Valor solicitado para antecipação

requestDate	      DateTime	  Data/hora da solicitação

status	string	  Status     (Pending, Approved, Rejected)

feeAmount	        decimal	    Valor da taxa (5%)

netAmount (v2)	  decimal	    Valor líquido após a taxa (somente retornado na versão 2 da API)

-----------------------------------------------------------------------------------

🔍 Exemplos de cURL (com JWT)

Use este token de exemplo (mock):

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo

-----------------------------------------------------------------------------------

Criar solicitação (v1)

curl -X POST http://localhost:8080/api/v1/AdvanceRequests \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo" \
  -d '{"creatorId":"user-123","requestedAmount":2000}'
---
Buscar solicitação por ID (v2)
curl -X GET http://localhost:8080/api/v2/AdvanceRequests/1 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo"
---
Buscar por criador (v1)
curl -X GET http://localhost:8080/api/v1/AdvanceRequests/creator/user-123 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo"
---
Aprovar solicitação (v2)
curl -X PUT http://localhost:8080/api/v2/AdvanceRequests/1/approve \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo"
---
Rejeitar solicitação (v1)
curl -X PUT http://localhost:8080/api/v1/AdvanceRequests/1/reject \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo"
---
Simular solicitação (v2)
curl -X GET "http://localhost:8080/api/v2/AdvanceRequests/simulate?requestedAmount=3000" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjcmVhdG9yMTkiLCJqdGkiOiIyYzBjNDY0ZS0yYmIwLTRhODctYjkxNC1lMGE2ZjBmMTgwMmMiLCJpYXQiOjE3NTc1NTYyMTYsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzU3NTU2MjE2LCJleHAiOjE3NTc1NTk4MTYsImlzcyI6Ikxhc3RMaW5rQXBpIiwiYXVkIjoiTGFzdExpbmtBcGkifQ.qp55K_hH1KkkamgBAGowhA9Jp7lNxrHNuni69tjnslo"

-----------------------------------------------------------------------------------

📂 Estrutura de Pastas
LastLinkApi.sln
├── LastLinkApi.Api            # Endpoints, Controllers, Swagger
├── LastLinkApi.Application    # Commands, Queries, Handlers
├── LastLinkApi.Domain         # Entidades, ValueObjects, Regras
├── LastLinkApi.Infrastructure # DbContext, Repositórios
└── LastLinkApi.Tests          # Testes unitários e de integração


