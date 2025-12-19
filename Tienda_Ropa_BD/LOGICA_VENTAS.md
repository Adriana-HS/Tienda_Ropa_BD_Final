# 📋 LÓGICA DEL FLUJO DE VENTAS

## 🔄 PROCESO COMPLETO DE UNA VENTA:

### **PASO 1: Seleccionar Cliente**
- El usuario selecciona un cliente del ComboBox
- El botón "Nuevo Pedido" se habilita automáticamente

### **PASO 2: Crear Pedido**
- Al hacer clic en "🛒 Nuevo Pedido":
  - Se crea un pedido vacío en la base de datos (estado: 'P' = Pendiente)
  - El pedido se crea con total = 0
  - El carrito está vacío inicialmente (por eso no se ve nada)
  - Se habilitan los botones: Agregar, Finalizar Venta, Cancelar Pedido, Actualizar Estado
  - Se bloquea la selección de cliente

**¿Por qué el carrito está vacío al crear el pedido?**
- Porque el pedido es solo un "contenedor" vacío
- Los productos se agregan DESPUÉS de crear el pedido
- Es como abrir una caja vacía antes de poner cosas dentro

### **PASO 3: Agregar Productos al Carrito**
Hay DOS formas de agregar productos:

**Opción A: Botón Agregar**
1. Seleccionar un producto de la lista
2. Ingresar la cantidad deseada
3. Hacer clic en "➕ Agregar"
4. El producto aparece en el carrito (lado derecho)

**Opción B: Doble Clic (Rápido)**
1. Hacer doble clic en cualquier producto
2. Se agrega automáticamente con cantidad = 1
3. Aparece inmediatamente en el carrito

**Características del Carrito:**
- Muestra: Producto, Cantidad, Precio Unitario, Descuento, Subtotal
- La columna "Cant." es EDITABLE (hacer clic para editar)
- Al editar la cantidad, se actualiza automáticamente en la base de datos
- Los totales se calculan automáticamente

### **PASO 4: Editar Cantidades (Opcional)**
- Hacer clic en la celda de "Cant." en el carrito
- Cambiar el número
- Presionar Enter o hacer clic fuera
- El sistema actualiza automáticamente:
  - Elimina el item con la cantidad anterior
  - Agrega el mismo producto con la nueva cantidad
  - Recalcula totales

### **PASO 5: Finalizar Venta**
- Al hacer clic en "✅ Finalizar Venta":
  - Cambia el estado del pedido a 'C' (Confirmado/Pagado)
  - Muestra la factura completa
  - Resetea todo para empezar una nueva venta

### **PASO 6: Actualizar Estado (Opcional)**
- El botón "🔄 Actualizar Estado" permite cambiar el estado manualmente:
  - **P** = Pendiente (pedido en proceso)
  - **C** = Confirmado/Pagado (venta completada)
  - **E** = Enviado (pedido enviado al cliente)

## 🎯 RESUMEN DEL FLUJO:

```
1. Seleccionar Cliente
   ↓
2. Clic en "Nuevo Pedido" → Se crea pedido vacío (carrito vacío es normal)
   ↓
3. Agregar productos (botón o doble clic) → Aparecen en el carrito
   ↓
4. (Opcional) Editar cantidades haciendo clic en la columna "Cant."
   ↓
5. Clic en "Finalizar Venta" → Cambia a estado 'C' y muestra factura
```

## ⚠️ IMPORTANTE:
- **Al crear pedido, el carrito está vacío** → Esto es CORRECTO y NORMAL
- Los productos se agregan DESPUÉS de crear el pedido
- El pedido es solo un "contenedor" que se llena con productos



