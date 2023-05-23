<template>
  <Navbar />
  <Product :loadProducts="loadProducts" :products="products" />
  <div class="pagination">
    <a @click="changePage(currentPage > 0 ? currentPage -= 1 : 1)">&laquo;</a>
    <a v-for="pageNumber in maxPages" :key="pageNumber" :class="{ active: pageNumber === currentPage }"
      @click="changePage(pageNumber)">{{ pageNumber }}</a>
    <a @click="changePage(currentPage < maxPages ? currentPage += 1 : maxPages)">&raquo;</a>
  </div>
</template>
<style>
.pagination {
  display: inline-block;
}

.pagination a {
  color: black;
  float: left;
  padding: 8px 16px;
  text-decoration: none;
}

.pagination a.active {
  background-color: #4CAF50;
  color: white;
}

.pagination a:hover:not(.active) {
  background-color: #ddd;
}
</style>

<script>
import Navbar from '../components/Navbar.vue'
import Product from '../components/Product.vue'
import axios from 'axios'
export default {
  components: {
    Navbar,
    Product
  }, data() {
    return {
      products: [],
      pageNum: 1,
      currentPage: 1,
      maxPages: 0
    };
  },
  methods: {
    changePage(pageNumber) {
      this.pageNum = pageNumber
      this.currentPage = pageNumber
      this.loadProducts()
    },
    loadProducts() {
      axios(this.base_url + "Products/" + this.pageNum, {
        method: 'GET',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
      }
      ).then(res => {
        this.products = res.data.query
        this.maxPages = res.data.totalPageCount
      }).catch(error => {
        console.log(error)
      })
    }
  }
}
</script>

