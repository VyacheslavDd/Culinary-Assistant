import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import { Collection } from 'types/collections.type';
import { transformCreatedAt } from 'utils/transform';
import { ButtonWrapper } from 'components/common';

type props = {
    collection: Collection;
};

export function OtherCollHeader(props: props) {
    const { collection } = props;

    const handleFavourite = () => {
        console.log('Добавить в избранное');
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
                                    src={collection.user.pictureUrl}
                                    alt='profile'
                                    className={styles.img}
                                />
                                {collection.user.login}
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
                    <div className={styles.description}>
                        <div className={styles.shareContainer}>
                            <ButtonWrapper
                                onAuthenticatedAction={handleFavourite}
                            >
                                <button className='button'>
                                    <span
                                        className={`icon ${styles.like}`}
                                    ></span>
                                    Нравится
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
