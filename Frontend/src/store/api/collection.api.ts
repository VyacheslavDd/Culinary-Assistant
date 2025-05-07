import axios from 'axios';
import qs from 'qs';
import { Category, CookingDifficulty, Tag } from 'types';
import { Collection } from 'types/collections.type';
import { ShortCollection } from 'types/short-collection.type';

const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000/';

export type TCollectionsData = {
    Page: number;
    Limit?: number;
    Title?: string;
};

export type TCollectionsResponse = {
    data: ShortCollection[];
    entitiesCount: number;
    pagesCount: number;
};

//Получение списка всех коллекций
export const getCollectionsApi = async (
    data: TCollectionsData
): Promise<TCollectionsResponse> => {
    try {
        const params = Object.fromEntries(
            Object.entries(data).filter(
                ([_, value]) =>
                    value !== undefined &&
                    value !== null &&
                    value !== '' &&
                    value !== 0 &&
                    (!Array.isArray(value) || value.length > 0)
            )
        );

        const response = await axios.get<TCollectionsResponse>(
            `${apiUrl}api/receipt-collections/all/by-filter`,
            {
                params: params,
                paramsSerializer: (params) =>
                    qs.stringify(params, { arrayFormat: 'repeat' }),
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true
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

            throw new Error('Getting recipes collections going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Получение отдельной коллекции
export const getCollectionByIdApi = async (id: string): Promise<Collection> => {
    try {
        const response = await axios.get<Collection>(
            `${apiUrl}api/receipt-collections/${id}`,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true
            }
        );
        const data = response.data;
        const collection: Collection = {
            ...data,
            receipts: data.receipts.map((receipt) => {
                return {
                    ...receipt,
                    category:
                        typeof receipt.category === 'string'
                            ? Category[
                                  receipt.category as keyof typeof Category
                              ]
                            : Category.hot,
                    cookingDifficulty:
                        typeof receipt.cookingDifficulty === 'string'
                            ? CookingDifficulty[
                                  receipt.cookingDifficulty as keyof typeof CookingDifficulty
                              ]
                            : CookingDifficulty.easy,
                    tags: Array.isArray(receipt.tags)
                        ? (receipt.tags
                              .map((value) => Tag[value as keyof typeof Tag])
                              .filter(Boolean) as Tag[])
                        : [],
                };
            }),
        };

        return collection;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting collection going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

//Получение списка коллекций у пользователя
export const getCollectionsByUserApi = async (data: {
    userId: string;
}): Promise<TCollectionsResponse> => {
    try {
        const response = await axios.get<TCollectionsResponse>(
            `${apiUrl}api/receipt-collections/all/by-filter`,
            {
                params: data,
                paramsSerializer: (params) =>
                    qs.stringify(params, { arrayFormat: 'repeat' }),
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true
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

            throw new Error('Getting recipes collections going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Удаление подборки рецептов
export const deleteCollectionApi = async (id: string): Promise<string> => {
    try {
        const response = await axios.delete(
            `${apiUrl}api/receipt-collections/${id}`,
            {
                headers: {
                    Accept: '*/*',
                },
                withCredentials: true
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

            throw new Error('Deleting collection failed');
        }

        throw new Error('Unknown error occurred');
    }
};

//Редактирование коллекции
export type UpdateCollectionDto = {
    title?: string;
    isPrivate?: boolean;
    color?: string;
};

export const updateCollectionApi = async (
    id: string,
    data: UpdateCollectionDto
): Promise<void> => {
    try {
        await axios.put(`${apiUrl}api/receipt-collections/${id}`, data, {
            headers: {
                Accept: '*/*',
                'Content-Type': 'application/json',
            },
            withCredentials: true
        });
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Updating collection failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Поставить лайк коллекции
export const likeCollectionApi = async (id: string): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipt-collections/${id}/likes`,
            {},
            {
                headers: {
                    Accept: '*/*',
                },
                withCredentials: true
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

            throw new Error('Adding to favorites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

export type newCollectionType = {
    title: string;
    isPrivate: boolean;
    color: string;
    userId: string;
};

// Создание новой подборки
export const createCollectionApi = async (
    newCollection: newCollectionType
): Promise<string> => {
    try {
        const response = await axios.post(
            `${apiUrl}api/receipt-collections`,
            newCollection,
            {
                headers: {
                    Accept: '*/*',
                    'Content-Type': 'application/json',
                },
                withCredentials: true
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
