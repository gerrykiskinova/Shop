<template>
    <Navbar />
    <h1>Your products</h1>
    <Product :loadProducts="loadUserProducts" :products="products" />
    <h1>Your orders</h1>
    <table>
        <thead>
            <tr>
                <th>Order name</th>
                <th>Total amount</th>
                <th>Options</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="order in orders">
                <td>{{ order.orderName }}</td>
                <td>{{ order.totalAmount }}$</td>
                <td>
                    <button class="button_two" @click="$event => deleteOrder(order.id)">delete</button>
                </td>
            </tr>
        </tbody>

    </table>
</template>

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
            orders: []
        };
    },
    mounted() {
        this.loadOrders()
    },
    methods: {
        loadUserProducts() {
            axios(this.base_url + "Products/user-products", {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(res => {
                this.products = res.data.query
                console.log(res.data.query)
            }).catch(error => {
                console.log(error)
            })
        },
        deleteOrder(id) {
            axios(this.base_url + "Orders/" + id, {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(() => {
                this.loadOrders()
            }).catch(error => {
                console.log(error)
            })
        },
        loadOrders() {
            axios(this.base_url + "Orders", {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(res => {
                this.orders = res.data
                console.log(res.data)
            }).catch(error => {
                console.log(error)
            })
        }
    }
}
</script>
  
  