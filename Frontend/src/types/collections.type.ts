import { ShortRecipe } from './short-recipe.type';
import { ShortUser } from './short-user.type';

export type Collection = {
    id: string;
    title: string;
    isPrivate: boolean;
    isLiked: false;
    popularity: 0;
    covers: { url: string }[];
    user: ShortUser;
    receipts: ShortRecipe[];
    createdAt: string;
};
