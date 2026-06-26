# 💸 PagaRápido

Sistema de pagamentos via Pix com processamento assíncrono e notificações em tempo real.

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![Vue 3](https://img.shields.io/badge/Vue.js-3-4FC08D?logo=vuedotjs)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?logo=rabbitmq&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)

---

## 📸 Screenshots

<!-- Adicione prints aqui após rodar o projeto -->

| Tela | Preview |
|------|---------|
| Dashboard de Pedidos | ![Dashboard](docs/screenshots/dashboard.png) |
| Criação de Pedido | ![Novo Pedido](docs/screenshots/novo-pedido.png) |
| Pagamento Confirmado (real-time) | ![Pago](docs/screenshots/pagamento-confirmado.png) |
| RabbitMQ Management | ![RabbitMQ](docs/screenshots/rabbitmq.png) |

---

## 🏗️ Arquitetura

```
┌──────────────┐        ┌─────────────────┐       ┌──────────────────────┐
│   Vue 3 SPA  │──────▶ │  Pedidos API    │──────▶│      RabbitMQ        │
│  (Vite)      │◀────── │  (.NET 9)       │       │                      │
│              │SignalR │  + EF Core      │       └──────────┬───────────┘
└──────────────┘        │  + SignalR Hub  │                  │
                        └─────────────────┘                  │
                              ▲                              │
                              │ PagamentoConfirmado          │ PedidoCriado
                              │                              ▼
                       ┌──────┴──────────┐       ┌──────────────────────┐
                       │   PostgreSQL    │◀──────│  Pagamentos Worker   │
                       │   (dados)       │       │  (simula Pix)        │
                       └─────────────────┘       └──────────┬───────────┘
                                                            │
                                                            │ PagamentoConfirmado
                                                            ▼
                                                 ┌──────────────────────┐
                                                 │ Notificações Worker  │
                                                 │ (email/push)         │
                                                 └──────────────────────┘
```

### Fluxo Completo

1. **Usuário cria pedido** → Vue envia POST para a API
2. **API salva no PostgreSQL** e publica `PedidoCriadoEvent` no RabbitMQ
3. **Worker Pagamentos** consome o evento, simula processamento Pix (2-5s)
4. **Worker Pagamentos** publica `PagamentoConfirmadoEvent`
5. **API consome** o evento, atualiza o status no banco e notifica o front via **SignalR**
6. **Worker Notificações** consome o evento e envia notificação (log/email)
7. **Front atualiza em real-time** — toast verde aparece sem refresh

---

## 🛠️ Stack

| Camada | Tecnologia | Propósito |
|--------|-----------|-----------|
| Front-end | Vue 3 + Vite | SPA responsiva |
| API | ASP.NET Core 9 | REST + SignalR Hub |
| Workers | .NET 9 Worker Service | Processamento assíncrono |
| Mensageria | RabbitMQ + MassTransit | Comunicação entre serviços |
| Banco | PostgreSQL 16 + EF Core 9 | Persistência |
| Real-time | SignalR | Notificações WebSocket |
| Containers | Docker + Docker Compose | Ambiente local |

---

## 🚀 Rodando Localmente

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Subir infraestrutura

```bash
docker compose up -d
```

Isso sobe PostgreSQL e RabbitMQ. Acesse o painel do RabbitMQ em http://localhost:15672 (guest/guest).

### 2. Rodar o backend (3 terminais)

```bash
# Terminal 1 - API
dotnet run --project src/PagaRapido.Pedidos.Api

# Terminal 2 - Worker de Pagamentos
dotnet run --project src/PagaRapido.Pagamentos.Worker

# Terminal 3 - Worker de Notificações
dotnet run --project src/PagaRapido.Notificacoes.Worker
```

### 3. Rodar o front-end

```bash
cd front
npm install
npm run dev
```

### 4. Acessar

- **Front-end:** http://localhost:5173
- **API:** http://localhost:5000
- **RabbitMQ:** http://localhost:15672

---

## 📡 Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/pedidos` | Criar novo pedido |
| GET | `/api/pedidos` | Listar todos os pedidos |
| GET | `/api/pedidos/{id}` | Buscar pedido por ID |
| WS | `/hubs/pedido` | Hub SignalR (real-time) |

### Exemplo - Criar Pedido

```bash
curl -X POST http://localhost:5000/api/pedidos \
  -H "Content-Type: application/json" \
  -d '{
    "clienteNome": "Daniel",
    "clienteEmail": "daniel@email.com",
    "itens": [
      { "produto": "Camiseta", "quantidade": 2, "precoUnitario": 49.90 },
      { "produto": "Boné", "quantidade": 1, "precoUnitario": 29.90 }
    ]
  }'
```

---

## 🐳 Docker (produção)

Para buildar tudo containerizado:

```bash
docker compose up --build
```

Os Dockerfiles usam multi-stage build para imagens otimizadas (~80MB cada).

---

## 📁 Estrutura do Projeto

```
PagaRapido/
├── src/
│   ├── PagaRapido.Pedidos.Api/        # API REST + SignalR
│   │   ├── Controllers/               # Endpoints
│   │   ├── Consumers/                 # Consumer MassTransit (atualiza status)
│   │   ├── Data/                      # DbContext + EF Core
│   │   ├── Dtos/                      # Request/Response models
│   │   ├── Hubs/                      # SignalR Hub
│   │   └── Dockerfile
│   ├── PagaRapido.Pagamentos.Worker/  # Processa pagamentos Pix
│   │   ├── Consumers/                 # Consumer de PedidoCriado
│   │   └── Dockerfile
│   ├── PagaRapido.Notificacoes.Worker/# Envia notificações
│   │   ├── Consumers/                 # Consumer de PagamentoConfirmado
│   │   └── Dockerfile
│   └── PagaRapido.Shared/             # Entities, Enums, Events
│       ├── Entities/
│       ├── Enums/
│       └── Events/
├── tests/
│   ├── PagaRapido.Pedidos.Tests/
│   ├── PagaRapido.Pagamentos.Tests/
│   └── PagaRapido.Notificacoes.Tests/
├── front/                             # Vue 3 + Vite
│   └── src/
│       ├── components/                # FormPedido, ListaPedidos, Notificacao
│       ├── api.js                     # Axios config
│       └── signalr.js                 # SignalR connection
├── docker-compose.yml
└── PagaRapido.sln
```

---

## 🧪 Testes

```bash
dotnet test
```

---

## 🌐 Deploy

| Serviço | Plataforma | URL |
|---------|-----------|-----|
| Front-end | Vercel | `https://pagarapido.vercel.app` |
| API + Workers | Render | `https://pagarapido-api.onrender.com` |
| PostgreSQL | Neon | Managed |
| RabbitMQ | CloudAMQP | Managed |

<!-- Atualize as URLs após configurar o deploy -->

---

## 📋 Conceitos Demonstrados

- ✅ **Microserviços** — API, Workers independentes com responsabilidade única
- ✅ **Event-Driven Architecture** — Comunicação via eventos (MassTransit + RabbitMQ)
- ✅ **CQRS simplificado** — API publica comandos, Workers processam
- ✅ **Real-time** — SignalR para notificações WebSocket
- ✅ **Docker** — Multi-stage builds, Docker Compose
- ✅ **Clean Code** — Separação de concerns, DTOs, entities isoladas
- ✅ **Async/Await** — I/O não bloqueante em toda a stack
- ✅ **EF Core 9** — Code-First, Fluent API, auto-migration em dev

---

## 🗺️ Roadmap

- [ ] Testes unitários e de integração
- [ ] QR Code Pix visual no front
- [ ] Rate Limiting na API
- [ ] Health Checks + `/metrics`
- [ ] CI/CD com GitHub Actions
- [ ] Observabilidade (OpenTelemetry)

---

## 👤 Autor

**Daniel Hoffmann**

[![GitHub](https://img.shields.io/badge/GitHub-DanielHoffmannO-181717?logo=github)](https://github.com/DanielHoffmannO)

---

## 📄 Licença

Este projeto é open-source sob a licença [MIT](LICENSE).
