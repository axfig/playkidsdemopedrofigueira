# match3-test

**Introdução**

Olá, pessoal da Playkids!
Eu fiz este teste da forma que acredito ser mais adequada - imaginando o jogo como se fosse um produto completo e fechado.

Isso significa que trabalhei pensando em início, meio e fim. Assim como dei o melhor acabamento possível dentro do tempo em que me restringi a realizar o desenvolvimento (uma semana com meu tempo livre, aprox. 25h de trabalho.

Eu nunca fiz um match 3 antes, então tive que fazer algumas pesquisas de Game Design antes de botar a mão no código. As decisões e diretrizes oriundas desta pesquisa estão listadas logo abaixo.

Para mim é muito importante que o projeto tenha um bom acabamento, com animações responsivas e movimentação fluida. Gosto muito que o jogo seja bem "juicy" então caprichei nas animações de interface, que é uma das coisas que mais gosto de fazer.

Espero que vocês gostem!

**Metodologia e features**

- Jogo Feito em C# com Unity 2019.4.2f1

- Toda vez que o tabuleiro detecta que encontrou uma situação sem solução, ele se re-embaralha. Foram utilizados dois critérios para determinar se cada peça indivudualmente pode ser resolvida. Para tal ela precisa A) ter um par de seu mesmo tipo e ao mesmo tempo, na direção oposta, ter uma outra peça de seu mesmo tipo na vizinhança. B) estar cercada de 3 ou mais peças do mesmo tipo.

- Todos os rounds tem dois minutos e a pontuação mínima necessária aumenta conforme as fases passam. Além disso, novas peças são introduzidas da fase 01 até a fase 04. Gerando assim dois critérios de dificuldade

- Foram utilizados SOMENTE as sprites e sons do projeto.

- Funciona em landscape e portrait, com input configurado para operar em mobile também.

- Pontuação: A) Permite pontuação de 4+, gerando um bônus por cada peça extra destruída. B) Permite pontuação em T, ou seja, caso dois grupos de match se encontrem de forma perpendicular.

- Menu de pause que não permite olhar o tabuleiro.

