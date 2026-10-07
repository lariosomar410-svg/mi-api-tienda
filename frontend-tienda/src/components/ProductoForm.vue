<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  productoEditando: Object
})

const emit = defineEmits(['guardar', 'cancelar-edicion'])

const nombre = ref('')
const precio = ref('')
const mensajeError = ref('')

// Definimos la función limpiar ANTES de usarla
const limpiarFormulario = () => {
  nombre.value = ''
  precio.value = ''
  mensajeError.value = ''
  emit('cancelar-edicion')
}

// Observar si nos mandan un producto para editar
watch(() => props.productoEditando, (nuevoVal) => {
  if (nuevoVal) {
    nombre.value = nuevoVal.nombre
    precio.value = nuevoVal.precio
  } else {
    limpiarFormulario()
  }
}, { immediate: true })

const manejarEnvio = () => {
  mensajeError.value = ''
  if (!nombre.value.trim() || precio.value <= 0) {
    mensajeError.value = 'Ingresa un nombre válido y un precio mayor a 0.'
    return
  }

  emit('guardar', {
    id: props.productoEditando ? props.productoEditando.id : null,
    nombre: nombre.value,
    precio: precio.value
  })

  limpiarFormulario()
}
</script>

<template>
  <form @submit.prevent="manejarEnvio" class="form-card">
    <h3>{{ productoEditando ? 'Editar Producto' : 'Nuevo Producto' }}</h3>

    <div class="form-group">
      <label>Nombre del Producto:</label>
      <input v-model="nombre" type="text" placeholder="Ej: Teclado Mecánico" required />
    </div>

    <div class="form-group">
      <label>Precio ($):</label>
      <input v-model.number="precio" type="number" step="0.01" min="0.01" placeholder="Ej: 50.00" required />
    </div>

    <p v-if="mensajeError" class="error-msg">{{ mensajeError }}</p>

    <button type="submit" class="btn-submit">
      {{ productoEditando ? 'Guardar Cambios' : 'Agregar Producto' }}
    </button>

    <button v-if="productoEditando" type="button" class="btn-cancel" @click="limpiarFormulario">
      Cancelar
    </button>
  </form>
</template>

<style scoped>
.form-card { background: #fff; padding: 15px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.08); margin-bottom: 20px; }
.form-group { margin-bottom: 10px; display: flex; flex-direction: column; }
input { padding: 8px; border: 1px solid #ccc; border-radius: 4px; margin-top: 4px; }
button { width: 100%; padding: 10px; border: none; border-radius: 4px; font-weight: bold; cursor: pointer; margin-top: 5px; }
.btn-submit { background: #42b883; color: white; }
.btn-cancel { background: #888; color: white; }
.error-msg { color: #e74c3c; font-size: 13px; }
</style>