<template>
  <router-view />
</template>

<script>

import axios from 'axios'
export default {
  mounted() {
    axios(this.base_url + "Account/isAuth", {
      method: 'get',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    }
    ).catch(res => {
      if (res.response.status == 401) {
        this.$router.push({ path: '/login' })
      }
    })
  }
}
</script>
<style>
body {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;

}
</style>
