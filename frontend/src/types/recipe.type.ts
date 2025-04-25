import { recipeDifficulty } from './short-recipe.type';
import { ShortUser } from './short-user.type';

export type Recipe = {
    id: string;
    title: string;
    description: string;
    ingredients: string[];
    steps: string[];
    calories: number;
    tags: string[];
    difficulty: recipeDifficulty;
    cookingTime: number;
    mainImage: string;
    images: string[];
    popularity: number;
    author: ShortUser;
    rating: number;
}