--Somente alterações feitas no banco original

INSERT INTO PERFIL VALUES (1, 'Aluno')
INSERT INTO PERFIL VALUES (2, 'Visitante')
INSERT INTO PERFIL VALUES (3, 'Master')
INSERT INTO PERFIL VALUES (4, 'Administrador')
INSERT INTO PERFIL VALUES (5, 'Auxiliar de Eventos')
--
--ALTER TABLE USUARIO ALTER COLUMN SENHA VARCHAR(MAX) NOT NULL
--
--ALTER TABLE EVENTO ADD IMAGEM_URL VARCHAR(MAX) NOT NULL
--
--ALTER TABLE EVENTO ADD SALA VARCHAR(20) NOT NULL
--
--ALTER TABLE USUARIO ADD ATIVO INT NOT NULL
--ALTER TABLE EVENTO ADD ATIVO INT NOT NULL
--VALOR 1 PARA ATIVO, VALOR 0 PARA INATIVO

--06-10-2020
--ALTER TABLE EVENTO ADD VAGAS INT NULL

--opcional
--ALTER TABLE USUARIO ALTER COLUMN CELULAR VARCHAR(20) NULL

--29-10-2020
ALTER TABLE INSCRICAO ALTER COLUMN DATA_HORA_PARTICIPACAO DATETIME NULL
ALTER TABLE INSCRICAO ADD COD_VALIDADO INT NULL

---------------C#-Asp.Net--------------------
--Configuration.ProxyCreationEnabled = false;
