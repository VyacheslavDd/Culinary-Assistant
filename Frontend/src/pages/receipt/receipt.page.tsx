import {
    ButtonsReceipt,
    Ingredients,
    MainInfo,
    Ratings,
    Review,
    Reviews,
    Steps,
} from 'components/receipt';
import styles from './receipt.module.scss';
import { useNavigate, useParams } from 'react-router-dom';
import ScrollToTop from 'components/common/scrollToTop';
import { useEffect, useState } from 'react';
import { Recipe } from 'types';
import { getRecipeByIdApi, getRecipeRateApi, putRateRecipeApi } from 'store/api';
import { Preloader } from 'components/preloader';
import { useSelector } from 'store/store';
import { selectIsAuthenticated } from 'store/user.slice';

function ReceiptPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [recipe, setRecipe] = useState<Recipe | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const isAuth = useSelector(selectIsAuthenticated);
    const [currentRating, setCurrentRating] = useState(0);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        if (!isAuth) return;
        if (!recipe?.id) return;

        const loadUserRating = async () => {
            try {
                const rating = await getRecipeRateApi(recipe.id);
                setCurrentRating(rating.rate);
                console.log(currentRating);
            } catch (error) {
                console.error('Ошибка загрузки оценки:', error);
            }
        };

        loadUserRating();
    }, [recipe, isAuth]);

    const handleRate = async (rating: number) => {
        if (!isAuth || isLoading || !recipe?.id) return;
        
        try {
            setIsLoading(true);
            await putRateRecipeApi(recipe.id, rating);
            setCurrentRating(rating);
        } catch (error) {
            console.error('Ошибка сохранения оценки:', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        const fetchRecipe = async () => {
            try {
                if (!id) {
                    throw new Error('Recipe ID is missing');
                }
                const data = await getRecipeByIdApi(id);
                setRecipe(data);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Unknown error occurred'
                );
            } finally {
                setLoading(false);
            }
        };

        fetchRecipe();
    }, [id]);

    const handleClick = () => {
        navigate(-1);
    };

    if (loading) {
        return <Preloader />;
    }

    if (error) {
        return <div className={styles.error}>Error: {error}</div>;
    }

    if (!recipe) {
        return <div className={styles.error}>Recipe not found</div>;
    }

    return (
        <>
            <ScrollToTop />
            <div className={styles.background}>
                <div className={styles.mainContainer}>
                    <div className={styles.backContainer}>
                        <button className={styles.back} onClick={handleClick}>
                            <span className={styles.arrow}></span>Назад
                        </button>
                    </div>
                    <div className={styles.container}>
                        <MainInfo recipe={recipe} />
                        <div className={styles.infoContainer}>
                            <Steps steps={recipe.cookingSteps} />
                            <Ingredients ingredients={recipe.ingredients} />
                        </div>
                        <div className={styles.reviewContainer}>
                            <ButtonsReceipt
                                name={recipe.title}
                                recipeId={recipe.id}
                                isFavourite={recipe.isFavourited}
                            />
                            <Ratings currentRating={currentRating} onRate={handleRate} />
                            <Review />
                            <Reviews />
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}

export default ReceiptPage;
