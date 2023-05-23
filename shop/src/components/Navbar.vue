<template>
    <nav class="skew-menu">
        <ul>
            <li>
                <RouterLink :to="{ name: 'home' }" style="padding-right:20px">
                    Products
                </RouterLink>
            </li>
            <li>
                <RouterLink :to="{ name: 'profile' }" style="padding-right:20px">
                    My profile
                </RouterLink>
            </li>
            <li v-if="isAdmin">
                <RouterLink :to="{ name: 'users' }" style="padding-right:20px">
                    Users
                </RouterLink>
            </li>
            <li @click="openModal"><a>Shopping basket</a></li>
            <li @click="logOut"><a href="#">Log out</a></li>
        </ul>
    </nav>
    <div v-if="showModal" class="modal">
        <div class="product-info">
            <span style="margin-left: 650px;" @click="closeModal">&times;Close</span>
            <h1>My shopping cart</h1>
            <table>
                <thead>
                    <tr>
                        <th>Product image</th>
                        <th>Product name</th>
                        <th>Seller</th>
                        <th>Price</th>
                        <th>Quantity</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in shoppingCart">
                        <td>
                            <img :src="'https://localhost:7133' + item.productPic" alt="placehold"
                                style="width: 100px;height: 100px;" />
                        </td>
                        <td>{{ item.productName }}</td>
                        <td>{{ item.seller }}</td>
                        <td>{{ item.productPrice }}</td>
                        <td>{{ item.quantity }}</td>
                        <td>
                            <button class="button_two" @click="removeFromShopCart(item.id)">remove</button>
                        </td>
                    </tr>
                </tbody>
            </table>
            <button class="button_two" @click="createOrder">Make order</button>
            <div class="pagination">
                <a @click="changePage(currentPage > 0 ? currentPage -= 1 : 1)">&laquo;</a>
                <a v-for="pageNumber in maxPages" :key="pageNumber" :class="{ active: pageNumber === currentPage }"
                    @click="changePage(pageNumber)">{{ pageNumber }}</a>
                <a @click="changePage(currentPage < maxPages ? currentPage += 1 : maxPages)">&raquo;</a>
            </div>
        </div>

    </div>
</template>
  
<style scoped>
.skew-menu {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    background-color: #f0f0f0;
    z-index: 9999;
    text-align: center;
}

.skew-menu ul {
    display: inline-block;
    margin: 0;
    padding: 0;
    list-style: none;
    transform: skew(-25deg);
}

.skew-menu ul li {
    background: #fff;
    float: left;
    border-right: 1px solid #eee;
    box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
    text-transform: uppercase;
    color: #555;
    font-weight: bolder;
    transition: all 0.3s linear;
}

.skew-menu ul li:first-child {
    border-radius: 7px 0 0 7px;
}

.skew-menu ul li:last-child {
    border-right: none;
    border-radius: 0 7px 7px 0;
}

.skew-menu ul li:hover {
    background: #eee;
    color: tomato;
}

.skew-menu ul li a {
    display: block;
    padding: 1em 2em;
    color: inherit;
    text-decoration: none;
    transform: skew(25deg);
}

.modal {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.product-info {
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-top: 1rem;
}

.close {
    margin-left: 700px;
}

@media (max-width: 768px) {
    .skew-menu ul li {
        display: none;
    }

    .skew-menu ul li:first-child,
    .skew-menu ul li:last-child {
        display: block;
    }

    .modal {
        margin-top: 3rem;
    }
}
</style>
  
<script>
import axios from 'axios'
export default {
    data() {
        return {
            shoppingCart: [],
            showModal: false,
            isAdmin: false,
            pageNum: 1,
            currentPage: 1,
            maxPages: 0
        }
    },
    mounted() {
        this.isAdmin = localStorage.getItem('role') == 1 ? false : true
    },
    methods: {
        changePage(pageNumber) {
            this.pageNum = pageNumber
            this.currentPage = pageNumber
            this.getShoppingCart()
        },
        openModal() {
            this.getShoppingCart()
            this.showModal = true
        },
        closeModal() {
            this.showModal = false
        }, logOut() {
            localStorage.setItem('token', '')
            this.$router.push({ path: '/login' })
        },
        createOrder() {
            axios({
                method: 'POST',
                url: this.base_url + 'Orders',
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
				this.closeModal()
                this.getShoppingCart()

            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        },
        removeFromShopCart(id) {
            axios({
                method: 'DELETE',
                url: this.base_url + 'ShoppingCarts/' + id,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.getShoppingCart()

            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        },
        getShoppingCart() {
            axios({
                method: 'GET',
                url: this.base_url + 'ShoppingCarts/' + this.pageNum,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(response => {
                this.shoppingCart = response.data.query
                this.maxPages = response.data.totalPages
                console.log(this.shoppingCart)

            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        }
    }
}
</script>
  

  