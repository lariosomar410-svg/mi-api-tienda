<script setup>
import { ref, onMounted } from 'vue'
import ProductoForm from './components/ProductoForm.vue'
import ProductoList from './components/ProductoList.vue'
import LoginForm from './components/LoginForm.vue'

const productos = ref([])
const cargando = ref(true)
const productoEditando = ref(null)
const token = ref(localStorage.getItem('jwt_token') || '')

const URL_API = 'http://localhost:5240/api/productos'

// Guardar Token al hacer Login
const manejarLogin = (nuevoToken) => {
  token.value = nuevoToken
  localStorage.setItem('jwt_token', nuevoToken)
}

// Cerrar Sesión
const cerrarSesion = () => {
  token.value = ''
  localStorage.removeItem('jwt_token')
}

// GET: Lectura pública
const obtenerProductos = async () => {
  try {
    const res = await fetch(URL_API)
    productos.value = await res.json()
  } catch (err) {
    console.error('Error al obtener productos:', err)
  } finally {
    cargando.value = false;
  }
}

// POST / PUT: Protegidos con Token JWT
const guardarProducto = async (producto) => {
  const headers = {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token.value}`
  }

  try {
    if (producto.id) {
      const res = await fetch(`${URL_API}/${producto.id}`, {
        method: 'PUT',
        headers: headers,
        body: JSON.stringify({ nombre: producto.nombre, precio: producto.precio })
      })
      if (res.ok) {
        const index = productos.value.findIndex(p => p.id === producto.id)
        productos.value[index] = producto
        productoEditando.value = null
      } else if (res.status === 401) {
        alert('Sesión expirada o no autorizada. Inicia sesión de nuevo.')
        cerrarSesion()
      }
    } else {
      const res = await fetch(URL_API, {
        method: 'POST',
        headers: headers,
        body: JSON.stringify({ nombre: producto.nombre, precio: producto.precio })
      })
      if (res.ok) {
        const nuevo = await res.json()
        productos.value.push(nuevo)
      } else if (res.status === 401) {
        alert('Debes iniciar sesión para agregar productos.')
        cerrarSesion()
      }
    }
  } catch (err) {
    console.error('Error al guardar:', err)
  }
}

// DELETE: Protegido con Token JWT
const eliminarProducto = async (id) => {
  if (!confirm('¿Seguro que deseas eliminar este producto?')) return
  try {
    const res = await fetch(`${URL_API}/${id}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token.value}` }
    })
    if (res.ok) {
      productos.value = productos.value.filter(p => p.id !== id)
    } else if (res.status === 401) {
      alert('No tienes autorización para eliminar.')
      cerrarSesion()
    }
  } catch (err) {
    console.error('Error al eliminar:', err)
  }
}

onMounted(() => {
  obtenerProductos()
})
</script>

<template>
  <div class="container">
    <div class="header">
      <h2>Catálogo (.NET + Vue 3 JWT)</h2>
      <button v-if="token" @click="cerrarSesion" class="btn-logout">Cerrar Sesión</button>
    </div>

    <!-- Si no hay token, muestra el formulario de Login -->
    <LoginForm v-if="!token" @login-exitoso="manejarLogin" />

    <!-- Si hay token, muestra el formulario de edición/creación -->
    <ProductoForm 
      v-else
      :productoEditando="productoEditando" 
      @guardar="guardarProducto"
      @cancelar-edicion="productoEditando = null" 
    />

    <ProductoList 
      :productos="productos" 
      :cargando="cargando"
      @editar="prod => productoEditando = prod"
      @eliminar="eliminarProducto" 
    />
  </div>
</template>

<style>
body { font-family: sans-serif; background: #f4f4f9; margin: 0; padding: 20px; }
.container { max-width: 500px; margin: 0 auto; }
.header { display: flex; justify-content: space-between; align-items: center; }
.btn-logout { background: #e74c3c; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-weight: bold; cursor: pointer; }
</style>