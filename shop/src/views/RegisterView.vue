<template>
    <div class="wrapper">
        <h2>Registration</h2>
        <form action="#">
            <div class="input-box">
                <input type="text" v-model="username" placeholder="Enter your name" required>
            </div>
            <div class="input-box">
                <input type="text" v-model="email" placeholder="Enter your email" required>
            </div>
            <div class="input-box">
                <input type="password" v-model="password" placeholder="Create password" required>
            </div>
            <div class="input-box">
                <input type="password" v-model="reppassword" placeholder="Confirm password" required>
            </div>
            <div class="policy">
                <input type="checkbox" v-model="terms">
                <h3>I accept all terms & condition</h3>
            </div>
            <div class="error">
                {{ errorMessage }}
            </div>
            <div class="input-box button">
                <input @click.prevent="submitForm" type="Submit" value="Register Now">
            </div>
            <div class="text">
                <h3>Already have an account?
                    <RouterLink :to="{ name: 'login' }">
                        Login now
                    </RouterLink>
                </h3>
            </div>
        </form>
    </div>
</template>
<script>
import axios from 'axios'
export default {
    data() {
        return {
            username: '',
            email: '',
            password: '',
            reppassword: '',
            errorMessage: '',
            terms: false
        }
    },
    methods: {
        submitForm() {
            if (this.password === '' || this.username === '' || this.reppassword === '' || this.email === '') {
                this.errorMessage = 'No empthy credentials allowed!'
            }
            else if (this.password !== this.reppassword) {
                this.errorMessage = 'Different passwords!'
            }
            else if (!this.terms) {
                this.errorMessage = 'Accept terms!'
            }
            else {
                this.errorMessage = ''
                const formData = new FormData();
                formData.append('Id', 0);
                formData.append('Username', this.username);
                formData.append('Password', this.password);
                formData.append('Email', this.email);
                formData.append('RegistrationDate', '01.01.1111');
                formData.append('Role', 0);

                axios({
                    method: 'POST',
                    url: this.base_url + 'Account/register',
                    data: formData,
                    headers: {
                        'Accept': " */*",
                        'Content-Type': `multipart/form-data`,
                    },
                }).then(response => {
                    localStorage.setItem('token', response.data.token)
                    localStorage.setItem('role', response.data.role)
                    this.$router.push({ path: '/' })

                })
                    .catch(error => {
                        this.onSubmitError = 'Something went wrong!'
                        console.log(error)
                    });
            }
        }
    }
}
</script>
<style scoped>
.error {
    color: red;
    font-weight: bold;
}

.wrapper {
    position: relative;
    max-width: 430px;
    width: 100%;
    background: #fff;
    padding: 34px;
    border-radius: 6px;
    box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
    padding-right: 54px;
    border: 5px solid #5D54A4;
}

.wrapper h2 {
    position: relative;
    font-size: 22px;
    font-weight: 600;
    color: #333;
}

.wrapper h2::before {
    content: '';
    position: absolute;
    left: 0;
    bottom: 0;
    height: 3px;
    width: 28px;
    border-radius: 12px;
    background: #5D54A4;
}

.wrapper form {
    margin-top: 30px;
}

.wrapper form .input-box {
    height: 52px;
    margin: 18px 0;
}

form .input-box input {
    height: 100%;
    width: 100%;
    outline: none;
    padding: 0 15px;
    font-size: 17px;
    font-weight: 400;
    color: #333;
    border: 1.5px solid #C7BEBE;
    border-bottom-width: 2.5px;
    border-radius: 6px;
    transition: all 0.3s ease;
}

.input-box input:focus,
.input-box input:valid {
    border-color: #5D54A4;
}

form .policy {
    display: flex;
    align-items: center;
}

form h3 {
    color: #707070;
    font-size: 14px;
    font-weight: 500;
    margin-left: 10px;
}

.input-box.button input {
    color: #fff;
    letter-spacing: 1px;
    border: none;
    background: #5D54A4;
    cursor: pointer;
}

.input-box.button input:hover {
    background: #5D54A4;
}

form .text h3 {
    color: #333;
    width: 100%;
    text-align: center;
}

form .text h3 a {
    color: #5D54A4;
    text-decoration: none;
}

form .text h3 a:hover {
    text-decoration: underline;
}
</style>