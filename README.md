# 🏭 Documentação Completa do Sistema Industrial Siderúrgico (Full-Stack)

**Sistema:** Steel Industry Operational & Telemetry Dashboard  
**Backend:** .NET 9.0 (C# 13) — Arquitetura DDD sem ORM, JWT Customizado, Telemetria & Compactador  
**Frontend:** Angular 20 (`20.3.43`) — Arquitetura Modular Não-Standalone, Angular Material (Dark Metallurgy) & Highcharts Dashboards  
**Localização do Repositório:** `C:\Projeto\IA`  

---

## 🔑 1. Credenciais Pré-Cadastradas para Testes

O backend inicializa automaticamente com 3 perfis industriais na memória (`InMemoryUserRepository`), disponíveis para login imediato tanto na interface web quanto via Swagger/Postman:

| Perfil (Role) | Usuário (Login) | Senha | Nível de Permissão | Acesso no Frontend |
|---|---|---|---|---|
| **Administrador** | `admin` | `Admin@123` | Nível 3 (Acesso total) | Dashboard, Catálogo de Aços, Bancada de Compactação e **Gestão de Usuários** |
| **Engenheiro Metalurgista** | `engenheiro.silva` | `Engenheiro@123` | Nível 2 (Técnico / Qualidade) | Dashboard, Catálogo de Aços, Bancada de Compactação |
| **Operador de Aciaria** | `operador.aciaria` | `Operador@123` | Nível 1 (Operação / Consulta) | Dashboard, Catálogo de Aços, Bancada de Compactação |

> 💡 **Atalho no Frontend:** Na tela de login (`/auth/login`), existem botões rápidos com as contas de teste que preenchem usuário e senha em 1 clique!

---

## 🚀 2. Guia de Execução Rápida

Para testar a solução completa integrada em sua máquina local, abra dois terminais:

### 2.1. Terminal 1 — Iniciar o Backend .NET 9
```powershell
cd C:\Projeto\IA\SteelIndustry\src\SteelIndustry.API
dotnet run
```
* **URL da API:** `http://localhost:5000` (e `http://localhost:5192`)
* **Swagger UI Interativo:** `http://localhost:5000/swagger`
* **Endpoint de Ping / Status:** `http://localhost:5000/api/ping`

### 2.2. Terminal 2 — Iniciar o Frontend Angular 20
```powershell
cd C:\Projeto\IA\SteelIndustry.Client
npm start
```
* **URL da Aplicação Web:** `http://localhost:4200`
* O Angular CLI iniciará com Hot-Reload ativo.

---

## 🖥️ 3. Arquitetura do Frontend (Angular 20 Não-Standalone)

O cliente web foi concebido sob uma arquitetura modular estrita baseada em `NgModules` (`standalone: false`), garantindo desacoplamento por domínio, lazy loading em todas as rotas e reaproveitamento de componentes.

### 3.1. Estrutura de Diretórios
```
C:\Projeto\IA\SteelIndustry.Client\src\app\
├── core/                                    # Infraestrutura Singleton da aplicação
│   ├── core.module.ts                       # Provedores de interceptores e singleton guard
│   ├── guards/                              # auth.guard.ts, role.guard.ts
│   ├── interceptors/                        # auth.interceptor.ts, telemetry.interceptor.ts, error.interceptor.ts
│   ├── models/                              # auth.model.ts, user.model.ts, material.model.ts, compactor.model.ts, telemetry.model.ts
│   └── services/                            # auth.service.ts, telemetry.service.ts, material-api.service.ts, user-api.service.ts, specification-parser.service.ts
│
├── shared/                                  # Componentes, Pipes e Módulos reutilizáveis
│   ├── shared.module.ts                     # Exportador geral de componentes compartilhados
│   ├── material/material.module.ts          # Centralizador de módulos do Angular Material
│   ├── components/                          # navbar, sidebar, telemetry-widget, steel-badge, confirm-dialog
│   └── pipes/                               # bytes.pipe.ts, ratio.pipe.ts
│
├── features/                                # Módulos de Funcionalidade (Lazy Loading)
│   ├── auth/                                # /auth/login (Tela de login industrial)
│   ├── dashboard/                           # /dashboard (Highcharts Dashboards: Espectrometria, Tração, Telemetria)
│   ├── materials/                           # /materials (Tabela MatTable em array, Laudos e CRUD de aços)
│   ├── compactor/                           # /compactor (Bancada interativa de compressão Brotli/GZip e descompactação)
│   └── users/                               # /users (Gestão de usuários protegida por RoleGuard: Admin)
│
├── app-routing-module.ts                    # Rotas raiz com loadChildren
├── app-module.ts                            # Módulo Raiz com provideHttpClient(withInterceptorsFromDi()) e provideHighcharts()
└── app.ts / app.html / app.scss             # Shell responsivo com MatSidenav e barra inferior de telemetria
```

### 3.2. Funcionalidades de Destaque no Frontend
1. **Tema Industrial Dark Metallurgy:** Paleta em grafite escuro (`#121418`), superfícies elevadas em aço escovado e acentos em laranja/âmbar aciaria (`#ff9800`) e azul elétrico (`#58a6ff`).
2. **Dashboard com Highcharts Integrado:**
   - **Espectrometria Química:** Gráfico de colunas com gradiente de alta temperatura comparando teores de Carbono (C), Manganês (Mn), Silício (Si), Cromo (Cr), Níquel (Ni), Fósforo (P), Enxofre (S) e Molibdênio (Mo).
   - **Propriedades Mecânicas:** Gráfico de barras horizontais ilustrando Limite de Escoamento (MPa), Resistência à Tração (MPa), Dureza Brinell (HB) e Alongamento (%).
   - **Histórico de Telemetria:** Gráfico spline em tempo real alimentado a cada chamada HTTP feita ao backend.
3. **Barra de Telemetria em Tempo Real:** Widget no rodapé da página que captura os cabeçalhos HTTP customizados do .NET 9 (`X-Response-Time-Ms` e `X-Memory-Allocated-Bytes`) exibindo latência e consumo de memória do GC do servidor.
4. **Bancada de Compactação Bidirecional:** Permite testar os algoritmos Brotli, GZip e Minificação, calculando em tempo real o percentual de redução de bytes e testando a descompactação com validação de integridade.

---

## ⚙️ 4. Arquitetura do Backend (.NET 9 DDD)

O backend foi construído seguindo o padrão **Domain-Driven Design (DDD)** em 4 camadas com separação rígida de responsabilidades e sem dependência de ORM pesado:

```
C:\Projeto\IA\SteelIndustry\src\
├── SteelIndustry.Domain/          # Entidades, Enums, Interfaces de Repositório e de Serviços
│   ├── Entities/                  # Material (especificações em JsonElement flexível), User
│   ├── Enums/                     # UserRole (1=Operador, 2=Engenheiro, 3=Admin), CompressionStrategy
│   └── Repositories/              # IMaterialRepository, IUserRepository
│
├── SteelIndustry.Application/     # DTOs, Contratos de AppServices e Injeção de Dependências
│   ├── DTOs/                      # MaterialDto, UserDto, CompactedMaterialResponseDto, LoginDTOs
│   └── Services/                  # MaterialAppService, UserAppService, AuthAppService
│
├── SteelIndustry.Infrastructure/  # Implementação de Repositórios In-Memory, Segurança e Compressão
│   ├── Repositories/              # InMemoryMaterialRepository, InMemoryUserRepository (ConcurrentDictionary)
│   ├── Security/                  # JwtTokenGenerator (HMAC-SHA256), PasswordHasher (SHA256)
│   └── Services/                  # MaterialJsonCompactorService (Brotli, GZip, Minificação)
│
└── SteelIndustry.API/             # Controllers REST, Middlewares de Performance e Pipeline HTTP
    ├── Controllers/               # RootController, AuthController, MaterialsController, UsersController
    ├── Middlewares/               # PerformanceMetricsMiddleware, JwtRequestAnalyzerMiddleware
    └── Program.cs                 # Injeção nativa, CORS irrestrito para dev e OpenAPI/Swagger
```

### 4.1. Middlewares Customizados
* **`PerformanceMetricsMiddleware`:** Mede com precisão de nanossegundos (`Stopwatch`) e audita a alocação de memória do Garbage Collector (`GC.GetTotalMemory`), injetando os cabeçalhos:
  - `X-Response-Time-Ms`
  - `X-Memory-Allocated-Bytes`
* **`JwtRequestAnalyzerMiddleware`:** Valida manualmente tokens JWT, preenche `HttpContext.User` com claims e concede bypass automático para rotas públicas (`/`, `/swagger`, `/api/auth/login`, `/api/ping`) e métodos marcados com `[AllowAnonymous]`.

---

## 📋 5. Catálogo Completo de Endpoints da API

### 5.1. Rotas Públicas (Sem necessidade de Token)
| Método | Endpoint | Descrição |
|---|---|---|
| `GET` | `/` | Retorna metadados do servidor, telemetria e links da API |
| `GET` | `/api/ping` | Healthcheck rápido para validar conexão com o Frontend |
| `POST` | `/api/auth/login` | Autentica usuário e retorna JWT Bearer Token |
| `GET` | `/swagger` | Documentação interativa Swagger UI |

### 5.2. Rotas Protegidas (Exigem header `Authorization: Bearer <token>`)
| Método | Endpoint | Perfil Mínimo | Descrição |
|---|---|---|---|
| `GET` | `/api/materials` | Operador | Retorna array de materiais `ActionResult<MaterialDto[]>` |
| `GET` | `/api/materials/{id}` | Operador | Retorna detalhes do lote pelo ID |
| `GET` | `/api/materials/code/{code}` | Operador | Busca material pelo código siderúrgico (ex: `STL-1020-001`) |
| `POST` | `/api/materials` | Engenheiro | Cadastra novo lote e especificações metalúrgicas |
| `PUT` | `/api/materials/{id}` | Engenheiro | Atualiza informações de corrida e especificações |
| `DELETE` | `/api/materials/{id}` | Engenheiro / Admin | Exclui lote do catálogo |
| `POST` | `/api/materials/compact` | Operador | Executa compressão (Brotli, GZip ou Minificação) |
| `POST` | `/api/materials/decompact` | Operador | Descompacta payload Base64 para JSON legível |
| `GET` | `/api/users` | Admin | Retorna array de usuários cadastrados `UserDto[]` |
| `GET` | `/api/users/{id}` | Admin | Retorna usuário pelo ID |
| `POST` | `/api/users` | Admin | Cadastra novo colaborador e perfil de acesso |
| `PUT` | `/api/users/{id}` | Admin | Atualiza perfil, e-mail e status ativo/inativo |
| `DELETE` | `/api/users/{id}` | Admin | Remove usuário do sistema |

---

## 🧪 6. Materiais Siderúrgicos Pré-Cadastrados (Seeds)

| Código | Grau do Aço | Norma Técnica | Corrida (Heat #) | Composição Típica |
|---|---|---|---|---|
| `STL-1020-001` | **SAE 1020** | NBR NM 87 / ASTM A29 | `CR-2026-A12` | C: 0.20%, Mn: 0.45%, Si: 0.22% |
| `STL-1045-002` | **SAE 1045** | SAE J403 / ABNT 1045 | `CR-2026-B88` | C: 0.45%, Mn: 0.75%, Si: 0.25% |
| `STL-304-003` | **Inox AISI 304** | ASTM A240 | `CR-2026-SS09` | Cr: 18.20%, Ni: 8.10%, C: 0.05% |
| `STL-A36-004` | **ASTM A36** | ASTM A36M | `CR-2026-STR44` | C: 0.25%, Mn: 0.90%, Si: 0.20% |

---

## 🛠️ 7. Guia de Teste Passo a Passo

1. **Acessar a Aplicação:**
   - Abra `http://localhost:4200` no navegador.
2. **Efetuar Login:**
   - Clique no botão de atalho `Administrador` (ou digite `admin` / `Admin@123`).
   - Clique em **"AUTENTICAR NO SISTEMA"**.
3. **Explorar o Dashboard:**
   - Alterne entre os botões de lotes (`STL-1020-001`, `STL-1045-002`, `STL-304-003`, `STL-A36-004`).
   - Observe os gráficos de Espectrometria Química e Propriedades Mecânicas atualizando instantaneamente.
   - Observe a barra inferior e o terceiro gráfico registrando a latência e o consumo de memória do .NET 9.
4. **Testar a Bancada de Compactação:**
   - Acesse o menu lateral: **Bancada de Compactação**.
   - Escolha o algoritmo **Brotli** e clique em **"EXECUTAR COMPACTAÇÃO NO BACKEND"**.
   - Analise a taxa de economia (geralmente > 60%) e o tempo de execução em milissegundos.
   - Clique em **"Testar Descompactação"** para validar o round-trip bidirecional com integridade.
5. **Gerenciar Catálogo e Usuários:**
   - No menu **Catálogo de Aços**, visualize laudos completos clicando no ícone do olho e teste a adição/edição de lotes.
   - No menu **Gestão de Usuários**, teste a criação de novos usuários industriais e desative/ative usuários existentes.
