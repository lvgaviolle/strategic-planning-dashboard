# Decisões técnicas e escopo

## Objetivo

Demonstrar competências com C#, Blazor Web App (.NET 9), SQL, persistência local, regras de negócio e UI responsiva. Este projeto não foi extraído de sistemas de uma empresa: a implementação e os dados são independentes e fictícios.

## Componentes

- **Components/Pages/Home.razor:** apresentação de indicadores, filtros combinados e evolução histórica.
- **Components/Pages/Goals.razor:** formulário validado e gestão de metas.
- **Models/Goal.cs:** entidade de meta e validação de cadastro.
- **Services/GoalMetrics.cs:** resumo de indicadores com normalização do atingimento.
- **Services/PlanningStore.cs:** acesso ao SQLite com consultas parametrizadas, criação do esquema e conjunto inicial de dados.
- **tests/:** testes de unidade e operações de persistência com arquivos temporários.

## Por que SQLite?

A demonstração pode ser executada com apenas o SDK .NET, sem servidor de banco, sem conta externa e sem mensalidade. Os dados permanecem na máquina, em `App_Data/northstar.db` (arquivo ignorado pelo Git).

## Regras e interpretação

O progresso individual é calculado por `(realizado / previsto) × 100` para metas positivas. Metas com previsto igual a zero têm progresso zero. O resumo utiliza a média aritmética dos progressos individuais, com teto de 100% por meta. Isso **não** equivale a consolidar indicadores heterogêneos, nem substitui cálculo ponderado ou numerador/denominador exigido em cenários de negócio reais.

Status demonstrativos:

- Atingida: progresso de pelo menos 100%.
- Em atenção: de 80% até menos de 100%.
- Abaixo da meta: menos de 80%.

Todos os indicadores consideram que um resultado maior é melhor; métricas em que menor é melhor exigiriam uma regra diferente.

## Limitações e segurança

- Sem autenticação, autorização, auditoria ou multiusuário.
- Banco compartilhado por todos os visitantes da mesma instância do servidor.
- Restauração substitui os registros existentes; a interface pede confirmação.
- Sem mecanismo de migração estruturada do esquema.
- Sem teste de acessibilidade automatizado, teste E2E ou avaliação formal de carga.
- Visualização histórica baseada em dados fictícios e em seis meses de amostra.
- Não hospedar esta versão diretamente na internet.

## Próximas melhorias opcionais

Se o projeto for evoluir além do propósito de portfólio local, considerar autenticação e autorização, dados isolados por usuário, migrações, validações adicionais, testes E2E e regras específicas por tipo de indicador.

A publicação **do código** em repositório público não exige que o aplicativo seja hospedado.
