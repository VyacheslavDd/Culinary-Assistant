import axios from 'axios';
import qs from 'qs';
import { Category, CookingDifficulty, Tag } from 'types';
import { Collection } from 'types/collections.type';
import { ShortCollection } from 'types/short-collection.type';
import { getEnumValueByString } from 'utils/transform';

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
                withCredentials: true,
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
                            ? getEnumValueByString(
                                  Category,
                                  receipt.category
                              ) ?? Category.mainCourse
                            : Category.mainCourse,
                    cookingDifficulty:
                        typeof receipt.cookingDifficulty === 'string'
                            ? getEnumValueByString(
                                  CookingDifficulty,
                                  receipt.cookingDifficulty
                              ) ?? CookingDifficulty.easy
                            : CookingDifficulty.easy,
                    tags: Array.isArray(receipt.tags)
                        ? (receipt.tags
                              .map((tag) => getEnumValueByString(Tag, tag))
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
            withCredentials: true,
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

export type rateCollectionType = {
    rate: number;
}

//Получение оценки пользователя подборки
export const getCollectionRateApi = async (id: string): Promise<rateCollectionType> => {
    try {
        const response = await axios.get<rateCollectionType>(
            `${apiUrl}api/receipt-collections/${id}/rate`,
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

            throw new Error('Getting rate of collection going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Поставить оценку коллекции
export const putRateCollectionApi = async (
    id: string,
    rate: number
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipt-collections/${id}/rate`,
            { rate: rate },
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Putting rate get failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Добавить подборку в избранное
export const CollectionFavoriteApi = async (id: string): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipt-collections/${id}/favourite`,
            {},
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Adding to favourites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Удалить подборку из избранного
export const CollectionUnfavoriteApi = async (id: string): Promise<void> => {
    try {
        await axios.delete(
            `${apiUrl}api/receipt-collections/${id}/unfavourite`,
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Deleting from favourites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

type addRecipesDto = {
    receipts: string[];
};

// Добавить рецепты в подборку
export const addRecipesCollectionApi = async (
    id: string,
    data: addRecipesDto
): Promise<void> => {
    try {
        await axios.patch(
            `${apiUrl}api/receipt-collections/${id}/add-receipts`,
            data,
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Adding recipes getting failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Удалить рецепты из подборки
export const deleteRecipesCollectionApi = async (
    id: string,
    data: addRecipesDto
): Promise<void> => {
    try {
        await axios.patch(
            `${apiUrl}api/receipt-collections/${id}/remove-receipts`,
            data,
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Deleting recipes getting failed');
        }

        throw new Error('Unknown error occurred');
    }
};
