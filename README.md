# 🛒 Sistema de Gestión de Catálogo (Full-Stack)

Aplicación web Full-Stack diseñada bajo una arquitectura desacoplada, con un frontend reactivo en **Vue 3** y un backend RESTful seguro en **ASP.NET Core**.

## 🔗 Links del Proyecto
- 🌐 **Frontend en vivo (Netlify):** [Tu-Enlace-Netlify](https://tu-sitio.netlify.app)
- ⚙️ **API REST en vivo (Render):** [Tu-Enlace-Render](https://mi-api-tienda-rspe.onrender.com)

## 🛠️ Tecnologías Utilizadas

- **Frontend:** Vue 3 (Composition API), HTML/CSS, Fetch API / Axios.
- **Backend:** C# / ASP.NET Core Minimal APIs.
- **ORM & DB:** Entity Framework Core, SQLite.
- **Seguridad:** JWT (JSON Web Tokens) Bearer Authentication.
- **Control de Versiones & Deploy:** Git, GitHub, Netlify, Render.

## 🔑 Características Principales
- **Lectura Pública:** Visualización del catálogo sin necesidad de autenticación.
- **Autenticación JWT:** Emisión de tokens firmados tras inicio de sesión exitoso.
- **Operaciones Protegidas:** Solo usuarios autenticados pueden crear, editar o eliminar productos.
- **Despliegue Continuo (CI/CD):** Actualización automática en la nube tras cada `git push`.

## 🚀 Ejecución Local

### Backend (.NET)
```bash
cd backend
dotnet restore
dotnet run

###Frontend (Vue 3)
```Bash
cd frontend
npm install
npm run dev
cd backend
dotnet restore
dotnet run
