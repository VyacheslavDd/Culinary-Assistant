import styles from './catalog.module.scss';
import { CatalogCard } from './card/card';
import { arrayRecipesMock } from 'mocks/catalog.mock';

export function Catalog() {
    const recipes = arrayRecipesMock;
    return (
        <div className={styles.catalog}>
            {recipes.map((recipe) => (
                <CatalogCard key={recipe.id} recipe={recipe} />
            ))}
        </div>
    );
}
