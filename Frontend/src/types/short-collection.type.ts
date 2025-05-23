import { Color } from './color.enum';

export type ShortCollection = {
    id: string;
    title: string;
    isPrivate: boolean;
    isFavourited: boolean;
    popularity: number;
    covers: { url: string }[];
    receiptNames: string[];
    createdAt: string;
    rating: number;
    color: Color;
    userLogin: string;
};
