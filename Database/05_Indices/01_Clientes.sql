CREATE INDEX IX_Clientes_NumeroDocumento
ON comercial.Clientes(NumeroDocumento);
go
CREATE INDEX IX_Clientes_Correo
ON comercial.Clientes(Correo)
where Correo is not null;
go
CREATE INDEX IX_Clientes_PublicId
ON comercial.Clientes(PublicId);