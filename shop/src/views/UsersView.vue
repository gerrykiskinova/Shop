<template>
    <Navbar />
    <div>
        <div v-if="showModal" class="modal">
            <form @submit.prevent="submitForm" class="form">
                <span class="close" @click="closeModal">&times;Close</span>
                <div class="form-group">
                    <label for="username">Username</label>
                    <input type="text" id="username" v-model="userInfo.username" required>
                </div>
                <div class="form-group">
                    <label for="email">Email</label>
                    <input type="text" id="email" v-model="userInfo.email" required>
                </div>
                <div class="form-group">
                    <label for="password">Password</label>
                    <input type="text" id="password" v-model="userInfo.password" required>
                </div>
                <button type="submit" @click.prevent="editUser">Edit</button>
            </form>
        </div>
        <h1>Users</h1>
        <table>
            <thead>
                <tr>
                    <th>Username</th>
                    <th>Email</th>
                    <th>Password</th>
                    <th>Registration date</th>
                </tr>
            </thead>


            <tbody>
                <tr v-for="user in users">
                    <td>{{ user.username }}</td>
                    <td>{{ user.email }}</td>
                    <td>{{ user.password }}</td>
                    <td>{{ user.registrationDate.replace("T00:00:00", '') }}</td>
                    <td>
                        <button class="button_one" @click.prevent="$event => openModal(user)">edit</button>
                        <button class="button_two" @click="$event => deleteUser(user.id)">delete</button>
                    </td>
                </tr>


            </tbody>

        </table>
    </div>
    <div class="pagination">
        <a @click="changePage(currentPage > 0 ? currentPage -= 1 : 1)">&laquo;</a>
        <a v-for="pageNumber in maxPages" :key="pageNumber" :class="{ active: pageNumber === currentPage }"
            @click="changePage(pageNumber)">{{ pageNumber }}</a>
        <a @click="changePage(currentPage < maxPages ? currentPage += 1 : maxPages)">&raquo;</a>
    </div>
</template>
<script>
import Navbar from '../components/Navbar.vue'
import axios from 'axios'
export default {
    components: {
        Navbar
    },
    data() {
        return {
            users: [],
            userInfo: null,
            showModal: false,
            pageNum: 1,
            currentPage: 1,
            maxPages: 0
        }
    },
    mounted() {
        this.getUsers()
    },
    methods: {
        changePage(pageNumber) {
            this.pageNum = pageNumber
            this.currentPage = pageNumber
            this.getUsers()
        },
        getUsers() {
            axios(this.base_url + "Users/" + this.pageNum, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(res => {
                console.log(res.data)
                this.users = res.data.users
                this.maxPages = res.data.totalPageCount
            }).catch(error => {
                console.log(error)
            })
        },
        openModal(user) {
            this.userInfo = user
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
        },
        editUser() {
            const formData = new FormData();
            formData.append('Id', this.userInfo.id);
            formData.append('Username', this.userInfo.username);
            formData.append('Password', this.userInfo.password);
            formData.append('Email', this.userInfo.email);
            formData.append('RegistrationDate', this.userInfo.registrationDate);
            formData.append('Role', this.userInfo.role);

            axios({
                method: 'PUT',
                url: this.base_url + 'Users/',
                data: formData,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                console.log("done")
                this.getUsers()
                this.closeModal()

            })
                .catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });
        },
        deleteUser(id) {
            axios(this.base_url + "Users/" + id, {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }
            ).then(() => {
                this.getUsers()
            }).catch(error => {
                console.log(error)
            })
        }
    }
}
</script>
