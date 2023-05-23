<template>
    <div>
        <div v-if="showModal" class="modal">
            <form v-if="isEditProduct" @submit.prevent="submitEdit" class="form">
                <span class="close" @click="closeModal">&times;Close</span>
                <div class="form-group">
                    <label for="name">Product Name</label>
                    <input type="text" id="name" v-model="selectedProduct.productName" required>
                </div>
                <div class="form-group">
                    <label for="price">Product Price</label>
                    <input type="number" id="price" v-model="selectedProduct.productPrice" required>
                </div>
                <div class="form-group">
                    <label for="image">Product Image</label>
                    <input type="file" id="image" v-on:change="changeImage">
                </div>
                <button type="submit">Edit</button>
            </form>
            <form v-else-if="!showSingleProd" @submit.prevent="submitForm" class="form">
                <span class="close" @click="closeModal">&times;Close</span>
                <div class="form-group">
                    <label for="name">Product Name</label>
                    <input type="text" id="name" v-model="productName" required>
                </div>
                <div class="form-group">
                    <label for="price">Product Price</label>
                    <input type="number" id="price" v-model="productPrice" required>
                </div>
                <div class="form-group">
                    <label for="image">Product Image</label>
                    <input type="file" id="image" v-on:change="changeImage" required>
                </div>
                <button type="submit">Submit</button>
            </form>
            <form v-else class="product-info form">
                <span class="close" @click.prevent="closeModal">&times;Close</span>
                <img class="product-pic" :src="'https://localhost:7133' + this.selectedProduct.productPic"
                    alt="Product Image">
                <div class="product-details">
                    <div class="product-name">{{ this.selectedProduct.productName }}</div>
                    <div class="seller">Sold by: {{ this.selectedProduct.seller }}</div>
                    <div class="price">{{ this.selectedProduct.productPrice }}$</div>
                    <label for="quantity">Choose quantity</label>
                    <input type="number" id="quantity" v-model="quantity">
                    <button @click.prevent="addToShoppingBasket" class="add-to-basket">Add to Basket</button>
                </div>
            </form>
        </div>
        <table>
            <thead>
                <tr>
                    <th>Product image</th>
                    <th>Product name</th>
                    <th>Seller</th>
                    <th>Price</th>
                    <th>Controls<button @click="openModal">Add new product</button></th>
                </tr>
            </thead>


            <tbody>
                <tr v-for="product in products">
                    <td><img :src="'https://localhost:7133' + product.productPic" alt="placehold"
                            style="width: 100px;height: 100px;">
                    </td>
                    <td>{{ product.productName }}</td>
                    <td>{{ product.seller }}</td>
                    <td>{{ product.productPrice }}$</td>
                    <td>
                        <button class="button_one" @click.prevent="$event => openSingleProd(product)">view</button>
                        <button v-if="product.isCreatedByUser" class="button_one"
                            @click="$event => editProduct(product)">edit</button>
                        <button v-if="product.isCreatedByUser" class="button_two"
                            @click="$event => deleteProduct(product.id)">delete</button>

                    </td>
                </tr>


            </tbody>

        </table>
    </div>
</template>
<script>
import Navbar from '../components/Navbar.vue'
import axios from 'axios'
export default {
    props: ['loadProducts', "products"],
    components: {
        Navbar
    },
    mounted() {
        this.loadProducts()
    }, data() {
        return {
            showModal: false,
            isEditProduct: false,
            selectedProduct: null,
            productName: '',
            productPrice: '',
            quantity: 1,
            productImg: null,
            showSingleProd: false,
            selectedProduct: null
        };
    },
    methods: {
        addToShoppingBasket() {
            const formData = new FormData();
            formData.append('Id', 0);
            formData.append('UserId', 0);
            formData.append('ProductId', this.selectedProduct.id);
            formData.append('Quantity', this.quantity);
            axios({
                method: 'POST',
                url: this.base_url + 'ShoppingCarts',
                data: formData,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.quantity = 1
                this.showSingleProd = false
                this.closeModal()

            })
                .catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });
        },
        changeImage(event) {
            this.productImg = event.target.files[0]
        },
        openModal() {
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
            this.showSingleProd = false;
            this.isEditProduct = false;
        },
        openSingleProd(product) {
            this.selectedProduct = product
            this.showSingleProd = true
            this.openModal()
        },
        editProduct(product) {
            this.selectedProduct = product
            this.isEditProduct = true
            this.openModal()
        },
        deleteProduct(id) {
            axios(this.base_url + "Products/" + id, {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(() => {
                this.loadProducts()
            }).catch(error => {
                console.log(error)
            })
        },
        submitEdit() {
            const formData = new FormData();
            formData.append('Id', this.selectedProduct.id);
            formData.append('CreatorId', 0);
            formData.append('ProductName', this.selectedProduct.productName);
            formData.append('ProductPic', 'pic');
            formData.append('Price', this.selectedProduct.productPrice);
            formData.append('Image', this.productImg);

            axios({
                method: 'PUT',
                url: this.base_url + 'Products/',
                data: formData,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.productName = ''
                this.productPrice = ''
                this.productImg = null
                this.closeModal()
                this.loadProducts()


            })
                .catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });


        },
        submitForm() {

            this.errorMessage = ''
            const formData = new FormData();
            formData.append('Id', 0);
            formData.append('CreatorId', 0);
            formData.append('ProductName', this.productName);
            formData.append('ProductPic', 'pic');
            formData.append('Price', this.productPrice);
            formData.append('Image', this.productImg);

            axios({
                method: 'POST',
                url: this.base_url + 'Products/',
                data: formData,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.productName = ''
                this.productPrice = ''
                this.productImg = null
                this.closeModal()
                this.loadProducts()


            })
                .catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });

        }
    }
}
</script>
<style >
.product-info {
    display: flex;
    align-items: center;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: #fff;
}

.product-pic {
    width: 100px;
    height: 100px;
    margin-right: 10px;
}

.product-details {
    flex: 1;
}

.product-name {
    font-size: 18px;
    font-weight: bold;
    margin-bottom: 5px;
}

.seller {
    color: #888;
    margin-bottom: 5px;
}

.price {
    font-size: 16px;
    color: #333;
}

.add-to-basket {
    background-color: #3498db;
    color: #fff;
    padding: 8px 15px;
    font-size: 16px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    margin-top: 10px;
}

.add-to-basket:hover {
    background-color: #2980b9;
}

.modal {
    display: flex;
    align-items: center;
    justify-content: center;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    /* Semi-transparent background */
    z-index: 9999;
}

/* Form style */
.form {
    position: relative;
    background-color: #fff;
    padding: 20px;
    width: 400px;
    border-radius: 4px;
}

/* Close button style */
.close {
    position: absolute;
    top: 10px;
    right: 10px;
    cursor: pointer;
}

/* Other form styles */
.form-group {
    margin-bottom: 15px;
}

label {
    display: block;
    font-weight: bold;
}

input[type="text"],
input[type="number"],
input[type="file"] {
    width: 100%;
    padding: 5px;
    border: 1px solid #ccc;
    border-radius: 4px;
    box-sizing: border-box;
}

button[type="submit"] {
    padding: 10px 20px;
    background-color: #4caf50;
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}

h2 {
    text-transform: capitalize;
    text-align: center;
}

table {
    width: 700px;
    margin: 20px auto;
    background-color: #eeeeee;
}

table,
th,
td {
    border: 1px solid white;
    border-collapse: collapse;
    padding: 15px;
    text-align: center;
    text-transform: capitalize;
    font-size: 20px;
}

table th {
    background-color: #404040;
    color: white;
}

img {
    border: 1px solid #b7b6b6;
}

button {
    font-size: 18px;
    padding: 8px;
    text-transform: capitalize;
    border-radius: 5px;
    border: none;
    cursor: pointer;
    -webkit-border-radius: 5px;
    -moz-border-radius: 5px;
    -ms-border-radius: 5px;
    -o-border-radius: 5px;
}

.button_one {
    background: #03a9f4;
    color: white;
}

.button_two {
    background: #e91e63;
    color: white;
}

.bottom {
    border-bottom: 2px solid #009688;
}
</style>