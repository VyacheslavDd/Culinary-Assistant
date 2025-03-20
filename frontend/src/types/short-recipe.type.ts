import { ShortUser } from './short-user.type';

export type ShortRecipe = {
    id: string;
    title: string;
    mainImage: string;
    description: string;
    author: ShortUser;
    cookingTime: number;
    difficulty: "easy" | "medium" | "hard";
    calories: number;
    tags: string[];
    popularity: number;
};