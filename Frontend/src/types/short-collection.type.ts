import { Color } from './color.enum';

export type ShortCollection = {
    id: string;
    title: string;
    isPrivate: boolean;
    isLiked: false;
    popularity: 0;
    covers: { url: string }[];
    receiptNames: string[];
    userLogin: string;
    createdAt: string;
    color: Color;
};
