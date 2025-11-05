# FCG_MS_User
Este microserviço faz parte do ecossistema FIAP Cloud Games e é responsável pelo cadastro, autenticação e gerenciamento de usuários. O projeto foi migrado de uma aplicação monolítica para uma arquitetura moderna, adotando Docker, integração contínua (CI/CD) e deployment em ambiente AWS.

## Principais Tecnologias
- .NET 8 – API estruturada em camadas de domínio, aplicação e infraestrutura

- Docker (multi-stage) – Build otimizado e imagem final baseada em aspnet:8.0

- GitHub Actions (CI/CD) – Build, testes e publicação automatizada no Amazon ECR

- AWS EC2 – Hospedagem da aplicação em container Docker

- AWS ECR – Registro das imagens do serviço de usuários

- Amazon RDS (PostgreSQL) – Banco de dados persistente em nuvem

- New Relic – Observabilidade, logs e monitoramento de performance

## Funcionalidades
- Cadastro e Gerenciamento de Usuários

- Registro de usuários com nome, e-mail e senha

- Validação de dados e senha forte

- Atualização e exclusão de contas

- Filtro e pesquisa por nome/e-mail

## Autenticação e Permissões

- Login com JWT (JSON Web Token)

- Controle de acesso por roles (Admin, User)

- Entidade dedicada UserAuthorization para mapear permissões

## Arquitetura

 - FCG_MS_User

    - Api – Controllers, Middlewares, Program.cs

    - Application – DTOs, Serviços e Interfaces

    - Domain – Entidades, Enums e Regras de Negócio

    - Infra – DbContext, Repositórios, Configurações de Persistência

✔️ Arquitetura em camadas seguindo boas práticas de DDD e REST

✔️ Injeção de dependência configurada via AddScoped

✔️ Estrutura pensada para evolução em microsserviços

## 🚀 CI/CD com GitHub Actions

- CI (Pull Request):

    - Build da solução

    - Execução dos testes unitários (dotnet test)

- CD (Merge para master):

    - Construção da imagem Docker
  
    - Publicação automática no Amazon ECR com tag latest

✅ Garantindo entregas consistentes, seguras e automatizadas.

## 📊 Monitoramento com New Relic
- Agent do New Relic instalado no container em execução na EC2

- Coleta de métricas: CPU, memória, throughput e latência

- Logs estruturados em JSON enviados ao New Relic Logs

- Dashboards monitorando erros, status codes e performance em tempo real

## ▶️ Como Rodar
1. Clone o repositório:
 ```bash
git clone https://github.com/leticiacarolinesilva/FCG_MS_User.git
 ```
2. Suba o ambiente local com Docker Compose (PostgreSQL incluso):
 ```bash
docker-compose up --build
```
3. Acesse o Swagger da API:
http://localhost:{port}/swagger/index.html

