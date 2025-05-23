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
import { Feedback, Recipe } from 'types';
import {
    createFeedbackApi,
    getFeedbacksApi,
    getRecipeByIdApi,
    getRecipeRateApi,
    putRateRecipeApi,
} from 'store/api';
import { Preloader } from 'components/preloader';
import { useSelector } from 'store/store';
import { selectIsAuthenticated } from 'store/user.slice';
import { Helmet } from 'react-helmet-async';

function ReceiptPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [recipe, setRecipe] = useState<Recipe | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const isAuth = useSelector(selectIsAuthenticated);
    const [currentRating, setCurrentRating] = useState(0);
    const [isLoading, setIsLoading] = useState(false);

    const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
    const [feedbacksLoading, setFeedbacksLoading] = useState(false);
    const [feedbacksError, setFeedbacksError] = useState<string | null>(null);

    const fetchFeedbacks = async () => {
        if (!recipe?.id) return;

        try {
            setFeedbacksLoading(true);
            const response = await getFeedbacksApi(recipe.id, {
                Page: 1,
            });
            setFeedbacks(response.data);
        } catch (err) {
            setFeedbacksError(
                err instanceof Error ? err.message : 'Unknown error'
            );
        } finally {
            setFeedbacksLoading(false);
        }
    };

    useEffect(() => {
        fetchFeedbacks();
    }, [recipe]);

    useEffect(() => {
        if (!isAuth) return;
        if (!recipe?.id) return;

        const loadUserRating = async () => {
            try {
                const rating = await getRecipeRateApi(recipe.id);
                setCurrentRating(rating.rate);
            } catch (error) {
                console.error('Ошибка загрузки оценки:', error);
            }
        };

        loadUserRating();
    }, [recipe, isAuth, currentRating]);

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

    const handleNewFeedback = async (text: string) => {
        if (!recipe?.id) return;

        try {
            await createFeedbackApi({
                receiptId: recipe.id,
                text,
            });
            fetchFeedbacks();
        } catch (error) {
            alert(
                error instanceof Error
                    ? `Ошибка добавления отзыва: ${error.message}`
                    : 'Ошибка добавления отзыва'
            );
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
            <Helmet>
                <title>{recipe.title}</title>
            </Helmet>
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
                            {isAuth && (
                                <Ratings
                                    currentRating={currentRating}
                                    onRate={handleRate}
                                />
                            )}
                            {isAuth && <Review onClick={handleNewFeedback} />}
                            <Reviews
                                feedbacks={feedbacks}
                                loading={feedbacksLoading}
                                error={feedbacksError}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}

export default ReceiptPage;
