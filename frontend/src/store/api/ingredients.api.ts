import axios from 'axios';
import { API_URL } from 'utils/variables';

const apiUrl = API_URL === 'undefined/' ? 'http://localhost:5000/' : API_URL;

// Получение списка ингредиентов
export const getIngredientsApi = async (): Promise<string[]> => {
    try {
        const response = await axios.get<string[]>(`${apiUrl}api/ingredients`, {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
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

            throw new Error('Getting recipes going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};
