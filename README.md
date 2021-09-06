# ContaBancaria
 Sistema de conta bancária para realizar operações básicas: Depósito, Saque, transferências e extratos
 
# Atividades
- [x] Subir MVP
- [x] Subir documentação 
- [ ] Revisão do código
- [ ] Revisão da documentação
- [ ] Testes unitários para os serviços de transferência, depósito e extrato
- [ ] Testes unitários para o serviço e autorização
- [ ] Adicionar JWT nas requisições
- [ ] Melhorar documentação do swagger

# Bancos de dados
Com a finalidade de facilitar a execução local do projeto, bancos de dados SQL Server em arquivo foram disponibilizados no diretório. E as configurações no arquivo Startup.cs permitem que as aplicações os encontrem e utilizem.
A única exceção feita foi relativa ao banco de dados em memória (cache). Para esse caso, foi realizada uma simulação utilizando uma classe estática.

# Autorização
Para o projeto foi idealizado um modelo de autenticação, que segue as seguintes premissas:
* Os endpoints só poderão ser acessados por dispositivos préviamente cadastrados. Para isso foi disponibilizado um endpoint que recebe um Guid identificador do dispositivo, e retorna um token de sessão. A sessão será utilizada em todas as outras chamadas a endpoints;
* Os endpoints de Saque e transferência necessitam do usuário e senha que esta utilizando o sistema, além do token de sessão. Com parâmetros válidos informados, ao final das transações de saque e transferência um endpoint de transação será acessado via sistema para validar as credênciais, finalizar a transação, e gerar um token de transação que permitirá rastrear todas as operações;
* Os serviços de extrato e depósito seguem o mesmo princípio descrito acima. Entretanto, o usuário e senha não é solicitado na interface do endpoint, e sim utilizadas credênciais do próprio sistema.

Ps: **Para que as transações ocorram, é imprescindível que o projeto de autorização esteja online**

# Endpoints
Para facilitar a execução e testes, as APIs também estão documentadas com Swagger.

* Serviço de Autorização
  * [Post] /api/Sessoes => Endpoint responsável por gerar o token de sessão. Como parâmetro é necessário informar a chave do dispositivo (Guid); 
  * [Delete] /api/Sessoes => Endpoint responsável por inativar o token de sessão, assim que o usuário finalizar as operações desejadas. Como parâmetro é necessário informar a chave de sessão (Guid).
  * [Post] /api/Transacoes => Responsável por autorizar as transações e gerar o token de sessão utilizado para rastrar as operações. É recomendado o uso de uma transacao para cada operação. Como parâmetro é necessário informar:
{
  "ChaveSessao": Guid,
  "Usuario": "string",
  "Senha": "string"
}
  * [Delete] /api/Transacoes => Responsável por inativar o token de transação

* Serviço de Transações
  * [Get] /api/Transacoes/extrato => Retorna todas as transações do usuário realizadas no período solicitado, limitado a 90 dias. Como parâmetros é necessário informar: Data de início e final da consulta (mm-dd-yyyy), token de sessão (Guid), numero da conta e agência. Como resultado teremos uma lista, contendo data, tipo de transação (saque, transferência, etc), valor, taxa, saldo, conta e agência (origem e destino);
  * [Post] api/Transacoes/deposito => Realiza o depósito de determinado valor na conta informada. Para essa transação foi configurada uma taxa proporcional sobre o valor depositado. Os seguintes parâmetros deverão ser informados: 
{
  "Valor": decimal,
  "NumeroConta": int,
  "Agencia": short,
  "Sessao": "3fa85f64-5717-4562-b3fc-2c963f66afa6" (exemplo)
}
  * [Post] api/Transacoes/saque => Realiza o saque de determinado valor na conta informada. Para essa transação foi configurada uma taxa fixa sobre o saldo disponível na conta. Caso a soma ValorDeSaque + Taxa > Saldo, a transação não será finalizada e retornará erro. Os seguintes parâmetros deverão ser informados: 
  * {
  "Usuario": "string",
  "Senha": "string",
  "BaseTransacao": {
    "Valor": decimal,
    "NumeroConta": int,
    "Agencia": short,
    "Sessao": "3fa85f64-5717-4562-b3fc-2c963f66afa6" (exemplo)
  }
  * [Post] api/Transacoes/transferencia => Realiza a transferência de determinado valor, entre as contas informadas (origem e destino). Para essa transação foi configurada uma taxa fixa sobre o saldo disponível na conta origem. Caso a soma ValorDeTransferência + Taxa > SaldoContaOrigem, a transação não finalizará, e retornará erro. Os seguintes parâmetros devem ser informados:
{
  "Usuario": "string",
  "Senha": "string",
  "Sessao": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Valor": 0,
  "NumeroContaOrigem": 0,
  "AgenciaOrigem": 0,
  "NumeroContaDestino": 0,
  "AgenciaDestino": 0,
  "CNPJBancoDestino": 0
}

**Todos os endpoints validam os parâmetros informados. Caso haja divergência, como informar valor de operação zero, contas não existentes no DB, sessão nula ou não existente, credênciais nulas ou inválidas, e etc, o sistema abortará a transação e retornará erro para o usuário**
