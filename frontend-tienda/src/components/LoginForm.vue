<script setup>
import { ref } from 'vue'

const emit = defineEmits(['login-exitoso'])

const usuario = ref('')
const password = ref('')
const cargando = ref(false)
const errorMensaje = ref('')

const URL_LOGIN = 'https://mi-api-tienda-rspe.onrender.com/api/login'

const iniciarSesion = async () => {
  if (!usuario.value || !password.value) {
    errorMensaje.value = 'Por favor completa todos los campos'
    return
  }

  cargando.value = true
  errorMensaje.value = ''

  try {
    const res = await fetch(URL_LOGIN, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        usuario: usuario.value,
        password: password.value
      })
    })

    if (res.ok) {
      const data = await res.json()
      // Emitimos el token al componente padre (App.vue)
      emit('login-exitoso', data.token)
    } else if (res.status === 401) {
      errorMensaje.value = 'Usuario o contraseña incorrectos'
    } else {
      errorMensaje.value = 'Ocurrió un error al intentar iniciar sesión'
    }
  } catch (err) {
    console.error('Error de conexión:', err)
    errorMensaje.value = 'No se pudo conectar con el servidor'
  } finally {
    cargando.value = false
  }
}
</script>

<template>
  <div class="login-card">
    <h3>Iniciar Sesión</h3>

    <form @submit.prevent="iniciarSesion">
      <div class="form-group">
        <label for="usuario">Usuario</label>
        <input 
          id="usuario"
          v-model="usuario" 
          type="text" 
          placeholder="Ej. admin" 
          :disabled="cargando"
        />
      </div>

      <div class="form-group">
        <label for="password">Contraseña</label>
        <input 
          id="password"
          v-model="password" 
          type="password" 
          placeholder="••••••••" 
          :disabled="cargando"
        />
      </div>

      <p v-if="errorMensaje" class="error-msg">{{ errorMensaje }}</p>

      <button type="submit" class="btn-login" :disabled="cargando">
        {{ cargando ? 'Ingresando...' : 'Entrar' }}
      </button>
    </form>
  </div>
</template>

<style scoped>
.login-card {
  background: #ffffff;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

.login-card h3 {
  margin-top: 0;
  color: #333;
}

.form-group {
  margin-bottom: 12px;
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: bold;
  margin-bottom: 4px;
  color: #555;
}

.form-group input {
  padding: 8px 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 0.95rem;
}

.btn-login {
  width: 100%;
  background: #2ecc71;
  color: white;
  border: none;
  padding: 10px;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  margin-top: 8px;
}

.btn-login:disabled {
  background: #95a5a6;
  cursor: not-allowed;
}

.error-msg {
  color: #e74c3c;
  font-size: 0.85rem;
  margin: 8px 0;
}
</style>