import axios from 'axios';
import { User } from 'types';
import { API_URL } from '../../../env';

const apiUrl = API_URL || 'http://localhost:5000/';

export type RegisterUserData = {
    login: string;
    emailOrPhone: string;
    password: string;
};

//Регистрация пользователя
export const registerUserApi = async (
    data: RegisterUserData
): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/register`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Register failed');
        }
        throw new Error('Unknown error occurred');
    }
};

export type AuthUserData = {
    login: string;
    password: string;
    rememberMe?: boolean;
};

//Аутентификация пользователя
export const loginUserApi = async (data: AuthUserData): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/login`,
            { adminEntrance: false, rememberMe: true, ...data },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Login failed');
        }
        throw new Error('Unknown error occurred');
    }
};

//Проверка авторизации пользователя
export const checkUserApi = async (): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/check-in`,
            {},
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Check-in failed');
        }
        throw new Error('Unknown error occurred');
    }
};

//Выход пользователя
export const logoutUserApi = async (): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/auth/logout`,
            {},
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Logout failed');
        }
        throw new Error('Unknown error occurred');
    }
};

export type UpdateUserDto = {
    login?: string;
    email?: string;
    phone?: string;
    profilePictureUrl?: string;
};

// Редактировать пользователя
export const updateUserApi = async (
    id: string,
    data: UpdateUserDto
): Promise<User> => {
    try {
        const response = await axios.put<User>(
            `${apiUrl}api/users/${id}`,
            data,
            {
                headers: {
                    Accept: '*/*',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Updating user failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Загрузка фотографии пользователя
export const uploadUserImage = async (file?: File): Promise<string> => {
    try {
        if (!file) {
            return '';
        }

        const formData = new FormData();
        formData.append('File', file); // именно 'File' — с заглавной буквы

        const response = await axios.post<{ url: string }>(
            `${apiUrl}api/files/users`,
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Accept: '*/*',
                },
                withCredentials: true,
            }
        );

        return response.data.url;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error('Ошибка загрузки изображения:', error.response?.data);
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Image user upload failed');
        }

        throw new Error('Unknown error occurred during image upload');
    }
};

// Удаление пользователя
export const deleteUserApi = async (id: string): Promise<string> => {
    try {
        const response = await axios.delete(`${apiUrl}api/users/${id}`, {
            headers: {
                Accept: '*/*',
            },
            withCredentials: true,
        });

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Deleting user failed');
        }

        throw new Error('Unknown error occurred');
    }
};
