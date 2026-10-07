<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  productos: Array,
  cargando: Boolean
})

const emit = defineEmits(['editar', 'eliminar'])

const busqueda = ref('')

const productosFiltrados = computed(() => {
  return props.productos.filter(p =>
    p.nombre.toLowerCase().includes(busqueda.value.toLowerCase())
  )
})
</script>

<template>
  <div class="list-card">
    <div class="form-group">
      <label>🔍 Buscar producto:</label>
      <input v-model="busqueda" type="text" placeholder="Escribe para filtrar..." class="search-input" />
    </div>

    <p v-if="cargando">Cargando productos desde .NET...</p>

    <ul v-else>
      <li v-for="prod in productosFiltrados" :key="prod.id">
        <span><strong>{{ prod.nombre }}</strong> — ${{ prod.precio }}</span>
        <div>
          <button class="btn-edit" @click="emit('editar', prod)">Editar</button>
          <button class="btn-delete" @click="emit('eliminar', prod.id)">Eliminar</button>
        </div>
      </li>
      <p v-if="productosFiltrados.length === 0 && !cargando">No se encontraron productos.</p>
    </ul>
  </div>
</template>

<style scoped>
.list-card { background: #fff; padding: 15px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.08); }
.search-input { width: 100%; padding: 8px; border: 1px solid #3498db; border-radius: 4px; margin-top: 4px; margin-bottom: 15px; box-sizing: border-box; }
li { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; padding-bottom: 5px; border-bottom: 1px solid #f0f0f0; }
button { padding: 5px 10px; border: none; border-radius: 4px; color: white; cursor: pointer; margin-left: 5px; font-size: 12px; font-weight: bold; }
.btn-edit { background: #3498db; }
.btn-delete { background: #e74c3c; }
</style>