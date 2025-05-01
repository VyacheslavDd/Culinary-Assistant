import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import { Toggle } from 'components/common';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import edit from '../../../assets/svg/edit.svg';
import { Collection } from 'types/collections.type';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { deleteCollectionApi } from 'store/api';

type props = {
    collection: Collection;
};

export function MyCollHeader(props: props) {
    const { collection } = props;
    const navigate = useNavigate();
    const [isDeleting, setIsDeleting] = useState(false);

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

    const handleDelete = async () => {
        if (
            !window.confirm(
                `Вы уверены, что хотите удалить подборку "${collection.title}"?`
            )
        ) {
            return;
        }

        try {
            setIsDeleting(true);
            await deleteCollectionApi(collection.id);
            navigate(-1);
        } catch (error) {
            alert((error as Error).message || 'Ошибка при удалении подборки');
        } finally {
            setIsDeleting(false);
        }
    };

    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <p className={styles.title}>{collection.title}</p>
                            <div className={styles.color}></div>
                        </div>
                        <div className={styles.buttons}>
                            <button className={styles.button}>
                                <img src={edit} alt='edit' />
                                Изм.
                            </button>
                            <div className={styles.publish}>
                                <Toggle isActive={!collection.isPrivate} />
                                <span>Опубликовать</span>
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
                    <div className={styles.shareContainer}>
                        <button className='button' onClick={handleShare}>
                            <span className={`icon ${styles.share}`}></span>
                            Поделиться
                        </button>
                        <button
                            className={`button ${styles.deleteButton} `}
                            onClick={handleDelete}
                            disabled={isDeleting}
                        >
                            <span className={`icon ${styles.delete}`}></span>
                            Удалить подборку
                        </button>
                    </div>
                </div>
            </LayoutHeader>
        </div>
    );
}
