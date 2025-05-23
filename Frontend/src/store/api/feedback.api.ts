import axios from 'axios';
import qs from 'qs';
import { Feedback } from 'types/feedback.type';
import { API_URL } from 'utils/variables';

const apiUrl = API_URL || 'http://localhost:5000/';

export type newFeedbackType = {
    receiptId: string;
    text: string;
};

// Создание отзыва
export const createFeedbackApi = async (
    newFeedback: newFeedbackType
): Promise<string> => {
    try {
        const response = await axios.post(
            `${apiUrl}api/feedbacks`,
            newFeedback,
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

            throw new Error('Failed to create collection');
        }

        throw new Error('Unknown error occurred');
    }
};

export type TFeedbacksData = {
    Page?: number;
    Limit?: number;
};

export type TFeedbacksResponse = {
    data: Feedback[];
    entitiesCount: number;
    pagesCount: number;
};

// Получение списка отзывов к рецепту
export const getFeedbacksApi = async (
    recipeId: string,
    data: TFeedbacksData
): Promise<TFeedbacksResponse> => {
    try {
        const params = Object.fromEntries(
            Object.entries(data).filter(
                ([_, value]) =>
                    value !== undefined &&
                    value !== null &&
                    value !== 0 &&
                    (!Array.isArray(value) || value.length > 0)
            )
        );

        const response = await axios.get<TFeedbacksResponse>(
            `${apiUrl}api/receipts/${recipeId}/feedbacks`,
            {
                params: params,
                paramsSerializer: (params) =>
                    qs.stringify(params, { arrayFormat: 'repeat' }),
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

            throw new Error('Getting feedbacks going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};
