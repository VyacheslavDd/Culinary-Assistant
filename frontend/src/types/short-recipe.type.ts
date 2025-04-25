export type ShortRecipe = {
    id: string;
    title: string;
    mainImage: string;
    cookingTime: number;
    difficulty: recipeDifficulty;
    calories: number;
    tags: string[];
    popularity: number;
    rating: number;
};

export type recipeDifficulty = 'easy' | 'medium' | 'hard';