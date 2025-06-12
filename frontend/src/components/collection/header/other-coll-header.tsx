import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import star from '../../../assets/svg/yellow_star.svg';
import { Collection } from 'types/collections.type';
import { transformCreatedAt, transformRating } from 'utils/transform';
import { ButtonWrapper } from 'components/common';
import default_user from '../../../assets/img/default-user.png';
import { useEffect, useState } from 'react';
import {
    CollectionFavoriteApi,
    CollectionUnfavoriteApi,
    getCollectionRateApi,
    putRateCollectionApi,
} from 'store/api';
import { Ratings } from 'components/receipt';
import { ReactComponent as Favorite } from '../../../assets/svg/fav.svg';
import { ReactComponent as Unfavorite } from '../../../assets/svg/unfav.svg';
import { useSelector } from 'store/store';
import { selectIsAuthenticated } from 'store/user.slice';

type props = {
    collection: Collection;
};

export function OtherCollHeader(props: props) {
    const { collection } = props;
    const [userRating, setUserRating] = useState(0);
    const [isRatingLoading, setIsRatingLoading] = useState(false);
    const [isFavourite, setIsFavourite] = useState<boolean>(
        collection.isFavourited
    );
    const [isFavouriteLoading, setIsFavouriteLoading] = useState(false);
    const isAuth = useSelector(selectIsAuthenticated);

    useEffect(() => {
        const loadUserRating = async () => {
            try {
                const ratingData = await getCollectionRateApi(collection.id);
                setUserRating(ratingData.rate);
            } catch (error) {
                console.error('Ошибка загрузки оценки:', error);
            }
        };

        loadUserRating();
    }, [collection.id]);

    const handleRateCollection = async (rating: number) => {
        try {
            setIsRatingLoading(true);
            await putRateCollectionApi(collection.id, rating);
            setUserRating(rating);
        } catch (error) {
            console.error('Ошибка при сохранении оценки:', error);
        } finally {
            setIsRatingLoading(false);
        }
    };

    const handleFavourite = async () => {
        if (!isAuth || isFavouriteLoading) return;

        try {
            setIsFavouriteLoading(true);

            if (isFavourite) {
                await CollectionUnfavoriteApi(collection.id);
                setIsFavourite(false);
            } else {
                await CollectionFavoriteApi(collection.id);
                setIsFavourite(true);
            }
        } catch (error) {
            console.error('Ошибка при изменении статуса избранного:', error);
        } finally {
            setIsFavouriteLoading(false);
        }
    };

    const handleShare = () => {
        const shareData = {
            title: collection.title,
            text: 'Посмотри эту потрясающую подборку рецептов!',
            url: window.location.href,
        };

        if (navigator.share) {
            navigator
                .share(shareData)
                .catch((err) =>
                    console.error('Ошибка при попытке поделиться:', err)
                );
        } else {
            navigator.clipboard
                .writeText(
                    `${shareData.title}
                ${shareData.url}`
                )
                .then(() => {
                    alert('Ссылка скопирована в буфер обмена!');
                });
        }
    };

    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <p className={styles.title}>{collection.title}</p>
                        </div>
                        <div className={styles.buttons}>
                            <div className={styles.profile}>
                                <img
                                    src={
                                        collection.user.pictureUrl === 'none'
                                            ? default_user
                                            : collection.user.pictureUrl
                                    }
                                    alt='profile'
                                    className={styles.img}
                                />
                                {collection.user.login}
                            </div>
                            <div className={styles.ratingContainer}>
                                <div className={styles.fav}>
                                    <img
                                        src={star}
                                        alt='rating'
                                        className={styles.icon}
                                    />
                                    <span>
                                        {transformRating(collection.rating)}
                                    </span>
                                </div>
                                <div className={styles.fav}>
                                    <img
                                        src={fav}
                                        alt='favorite'
                                        className={styles.icon}
                                    />
                                    <span>{collection.popularity}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    {isAuth && (
                        <Ratings
                            currentRating={userRating}
                            onRate={handleRateCollection}
                            disabled={isRatingLoading}
                            title={'подборку'}
                        />
                    )}

                    <div className={styles.description}>
                        <div className={styles.shareContainer}>
                            <ButtonWrapper
                                onAuthenticatedAction={handleFavourite}
                            >
                                <button
                                    className='button'
                                    disabled={isFavouriteLoading}
                                >
                                    {isFavourite ? (
                                        <Favorite className={`icon`} />
                                    ) : (
                                        <Unfavorite className={`icon`} />
                                    )}
                                    {isFavourite ? 'Не нравится' : 'Нравится'}
                                </button>
                            </ButtonWrapper>

                            <button className='button' onClick={handleShare}>
                                <span className={`icon ${styles.share}`}></span>
                                Поделиться
                            </button>
                        </div>
                        <p>{transformCreatedAt(collection.createdAt)}</p>
                    </div>
                </div>
            </LayoutHeader>
        </div>
    );
}
