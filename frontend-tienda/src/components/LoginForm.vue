<script setup>
import { ref } from 'vue'

const emit = defineEmits(['login-exitoso'])

const username = ref('')
const password = ref('')
const error = ref('')

const iniciarSesion = async () => {
  error.value = ''
  try {
    const res = await fetch('http://localhost:5240/api/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username: username.value, password: password.value })
    })

    if (res.ok) {
      const data = await res.json()
      emit('login-exitoso', data.token)
    } else {
      error.value = 'Usuario o contraseña incorrectos'
    }
  } catch (err) {
    error.value = 'Error al conectar con el servidor'
  }
}
</script>

<template>
  <div class="login-card">
    <h3>🔐 Iniciar Sesión (Admin)</h3>
    <form @submit.prevent="iniciarSesion">
      <div class="form-group">
        <label>Usuario:</label>
        <input v-model="username" type="text" placeholder="admin" required />
      </div>
      <div class="form-group">
        <label>Contraseña:</label>
        <input v-model="password" type="password" placeholder="123" required />
      </div>
      <p v-if="error" class="error-msg">{{ error }}</p>
      <button type="submit" class="btn-login">Entrar</button>
    </form>
  </div>
</template>

<style scoped>
.login-card { background: #fff; padding: 15px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.08); margin-bottom: 20px; border-left: 4px solid #3498db; }
.form-group { margin-bottom: 10px; display: flex; flex-direction: column; }
input { padding: 8px; border: 1px solid #ccc; border-radius: 4px; margin-top: 4px; }
.btn-login { width: 100%; padding: 10px; background: #3498db; color: white; border: none; border-radius: 4px; font-weight: bold; cursor: pointer; }
.error-msg { color: #e74c3c; font-size: 13px; }
</style>