# BankApi

##  Uso de la UI (paso a paso)

La UI está organizada en **tres pestañas**: **Cliente**, **Cuenta** y **Operaciones**.

### 1) Crear cliente
- Completa **Nombre**, **Fecha de nacimiento**, **Sexo** e **Ingresos**.
- Pulsa **Crear**.
- Debajo verás **Clientes guardados**.
- Cada fila muestra sus **cuentas** como “chips” con el **número de cuenta** (si tiene).

### 2) Crear cuenta
- Ingresa el **ID del cliente** (se autocompleta al crear uno).
- Define **Saldo inicial** y, si quieres, **Tasa de interés** (0–1).
- Pulsa **Crear cuenta**.

### 3) Operaciones con la cuenta
- Escribe o pega el **Número de cuenta**, o haz clic en un **chip** de “Clientes guardados”.
- **Consultar saldo**: botón **Consultar saldo** (refresca saldo e historial).
- **Depósito**: ingresa el monto y pulsa **Depositar**.
- **Retiro**: ingresa el monto y pulsa **Retirar**.  
  - Si no hay fondos suficientes, la operación se **rechaza**.
- **Historial**: se actualiza automáticamente tras cada operación; también puedes refrescarlo con el botón **Historial**.

### 4) Eliminar cliente
- En **Clientes guardados**, pulsa **Eliminar** en la fila del cliente.
- Se borran también sus **cuentas** y **transacciones**.
