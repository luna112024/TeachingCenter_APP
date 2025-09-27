/**
 * Modern Notification System for IBanking
 * Author: Kun Visal-Prompt Engineering
 * Version: 2.0 - Complete Redesign
 */

class ModernNotifyBox {
    constructor() {
        this.container = null;
        this.notifications = [];
        this.maxNotifications = 5;
        this.defaultDuration = 5000;
        this.init();
    }

    init() {
        this.createContainer();
        this.injectStyles();
    }

    createContainer() {
        if (this.container) return;

        this.container = document.createElement('div');
        this.container.id = 'modern-notify-container';
        this.container.className = 'modern-notify-container';
        document.body.appendChild(this.container);
    }

    injectStyles() {
        if (document.getElementById('modern-notify-styles')) return;

        const styles = document.createElement('style');
        styles.id = 'modern-notify-styles';
        styles.textContent = `
            .modern-notify-container {
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 10000;
                max-width: 420px;
                pointer-events: none;
            }

            .modern-notification {
                display: flex;
                align-items: flex-start;
                margin-bottom: 12px;
                padding: 16px 20px;
                background: #ffffff;
                border-radius: 12px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
                border-left: 4px solid;
                backdrop-filter: blur(10px);
                pointer-events: auto;
                transform: translateX(100%);
                opacity: 0;
                transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
                position: relative;
                overflow: hidden;
                min-height: 64px;
            }

            .modern-notification.show {
                transform: translateX(0);
                opacity: 1;
            }

            .modern-notification.hide {
                transform: translateX(100%);
                opacity: 0;
                margin-bottom: 0;
                padding-top: 0;
                padding-bottom: 0;
                min-height: 0;
            }

            .modern-notification.primary {
                border-left-color: #3b82f6;
                background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
            }

            .modern-notification.success {
                border-left-color: #10b981;
                background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
            }

            .modern-notification.warning {
                border-left-color: #f59e0b;
                background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
            }

            .modern-notification.error {
                border-left-color: #ef4444;
                background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
            }

            .modern-notification.info {
                border-left-color: #06b6d4;
                background: linear-gradient(135deg, #cffafe 0%, #a5f3fc 100%);
            }

            .notification-icon {
                flex-shrink: 0;
                width: 24px;
                height: 24px;
                margin-right: 12px;
                margin-top: 2px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 14px;
                font-weight: bold;
                color: white;
            }

            .notification-icon.primary { background: #3b82f6; }
            .notification-icon.success { background: #10b981; }
            .notification-icon.warning { background: #f59e0b; }
            .notification-icon.error { background: #ef4444; }
            .notification-icon.info { background: #06b6d4; }

            .notification-content {
                flex: 1;
                min-width: 0;
            }

            .notification-title {
                font-weight: 600;
                font-size: 14px;
                line-height: 1.4;
                margin-bottom: 4px;
                color: #1f2937;
            }

            .notification-message {
                font-size: 13px;
                line-height: 1.5;
                color: #6b7280;
                word-wrap: break-word;
            }

            .notification-close {
                flex-shrink: 0;
                width: 28px;
                height: 28px;
                margin-left: 12px;
                border: none;
                background: rgba(0, 0, 0, 0.1);
                border-radius: 50%;
                cursor: pointer;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 16px;
                color: #6b7280;
                transition: all 0.2s ease;
            }

            .notification-close:hover {
                background: rgba(0, 0, 0, 0.2);
                color: #374151;
                transform: scale(1.1);
            }

            .notification-progress {
                position: absolute;
                bottom: 0;
                left: 0;
                height: 3px;
                background: rgba(255, 255, 255, 0.3);
                border-radius: 0 0 12px 12px;
                overflow: hidden;
                width: 100%;
            }

            .notification-progress-bar {
                height: 100%;
                background: linear-gradient(90deg, #3b82f6, #1d4ed8);
                transition: width 0.1s linear;
                border-radius: 0 0 12px 12px;
                width: 100%;
            }

            .notification-progress.success .notification-progress-bar {
                background: linear-gradient(90deg, #10b981, #047857);
            }

            .notification-progress.warning .notification-progress-bar {
                background: linear-gradient(90deg, #f59e0b, #d97706);
            }

            .notification-progress.error .notification-progress-bar {
                background: linear-gradient(90deg, #ef4444, #dc2626);
            }

            /* Mobile Responsive */
            @media (max-width: 640px) {
                .modern-notify-container {
                    top: 10px;
                    right: 10px;
                    left: 10px;
                    max-width: none;
                }
            }

            /* Hover effects */
            .modern-notification:hover {
                transform: translateX(-4px);
                box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
            }
        `;
        document.head.appendChild(styles);
    }

    show(message, type = 'info', duration = null, options = {}) {
        this.cleanupOldNotifications();

        const config = {
            title: this.getTypeTitle(type),
            showProgress: true,
            closable: true,
            ...options
        };

        const notification = this.createNotification(message, type, duration || this.defaultDuration, config);
        this.notifications.push(notification);
        this.container.appendChild(notification.element);

        requestAnimationFrame(() => {
            notification.element.classList.add('show');
        });

        if (duration !== 0) {
            this.startAutoDismiss(notification, duration || this.defaultDuration);
        }

        return notification;
    }

    createNotification(message, type, duration, config) {
        const notification = document.createElement('div');
        notification.className = `modern-notification ${type}`;

        const iconSymbol = this.getIconSymbol(type);
        
        notification.innerHTML = `
            <div class="notification-icon ${type}">${iconSymbol}</div>
            <div class="notification-content">
                ${config.title ? `<div class="notification-title">${config.title}</div>` : ''}
                <div class="notification-message">${message}</div>
            </div>
            ${config.closable ? '<button class="notification-close" aria-label="Close">×</button>' : ''}
            ${config.showProgress && duration > 0 ? `
                <div class="notification-progress ${type}">
                    <div class="notification-progress-bar"></div>
                </div>
            ` : ''}
        `;

        const notificationObj = {
            element: notification,
            type: type,
            duration: duration,
            startTime: Date.now(),
            id: Date.now() + Math.random()
        };

        if (config.closable) {
            const closeBtn = notification.querySelector('.notification-close');
            closeBtn.addEventListener('click', () => this.dismiss(notificationObj));
        }

        return notificationObj;
    }

    getIconSymbol(type) {
        const icons = {
            success: '✓',
            error: '✕',
            warning: '⚠',
            info: 'ⓘ',
            primary: '●'
        };
        return icons[type] || icons.info;
    }

    getTypeTitle(type) {
        const titles = {
            success: 'Success',
            error: 'Error',
            warning: 'Warning',
            info: 'Information',
            primary: 'Notice'
        };
        return titles[type] || titles.info;
    }

    startAutoDismiss(notification, duration) {
        const progressBar = notification.element.querySelector('.notification-progress-bar');
        
        if (progressBar) {
            let startTime = Date.now();
            const updateProgress = () => {
                const elapsed = Date.now() - startTime;
                const progress = Math.max(0, 100 - (elapsed / duration) * 100);
                
                if (progress > 0) {
                    progressBar.style.width = `${progress}%`;
                    requestAnimationFrame(updateProgress);
                } else {
                    this.dismiss(notification);
                }
            };
            updateProgress();
        } else {
            setTimeout(() => this.dismiss(notification), duration);
        }
    }

    dismiss(notification) {
        if (!notification.element.parentNode) return;

        notification.element.classList.add('hide');
        
        setTimeout(() => {
            if (notification.element.parentNode) {
                notification.element.parentNode.removeChild(notification.element);
            }
            this.notifications = this.notifications.filter(n => n.id !== notification.id);
        }, 400);
    }

    cleanupOldNotifications() {
        if (this.notifications.length >= this.maxNotifications) {
            const oldNotification = this.notifications.shift();
            this.dismiss(oldNotification);
        }
    }

    clear() {
        this.notifications.forEach(notification => this.dismiss(notification));
        this.notifications = [];
    }
}

// Create global instance
const modernNotify = new ModernNotifyBox();

// Updated myNotifyBox function with modern design
function myNotifyBox(msg, duration = 5000, color = 'info') {
    const typeMap = {
        'primary': 'primary',
        'warning': 'warning', 
        'success': 'success',
        'error': 'error',
        'info': 'info'
    };

    const type = typeMap[color] || 'info';
    return modernNotify.show(msg, type, duration);
}

// Enhanced API for more control
window.modernNotify = modernNotify;
window.showSuccess = (msg, duration) => modernNotify.show(msg, 'success', duration);
window.showError = (msg, duration) => modernNotify.show(msg, 'error', duration);
window.showWarning = (msg, duration) => modernNotify.show(msg, 'warning', duration);
window.showInfo = (msg, duration) => modernNotify.show(msg, 'info', duration);

// Backward compatibility for existing code
$("body").on("click", ".notification-close", function () {
    // This is now handled by the modern notification system
});

// Keep existing legacy functions but mark them as deprecated
function AzisPrint(actionUrl, title, isAll) {
    console.warn('AzisPrint is deprecated. Consider using the new export library.');
    // ... existing code remains unchanged for backward compatibility
    var search = $("#SearchText").val();
    var url = isAll ? actionUrl : actionUrl + "? =" + search;
    $.ajax({
        url: url,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
    }).done(function (result) {
        const table = document.createElement("table");
        table.className = "table table-bordered";
        var header = table.createTHead();
        var tr = header.insertRow(-1);
        for (let i = 0; i < result.header.length; i++) {
            let th = document.createElement("th");
            th.innerHTML = result.header[i];
            tr.appendChild(th);
        }
        var body = table.createTBody();
        result.data.forEach((item) => {
            var tdr = document.createElement("tr");
            let vals = Object.values(item);
            vals.forEach((elem) => {
                let td = document.createElement("td");
                td.innerText = elem;
                tdr.appendChild(td);
            });
            body.appendChild(tdr);
        });

        var winPrint = window.open('', '', 'left=0,top=0,width=800,height=600,toolbar=0,scrollbars=0,status=0');
        winPrint.document.write("<html><head><title>" + title + "</title>");
        winPrint.document.write('<link rel="stylesheet" href="/assets/vendor/css/core.min.css" />');
        winPrint.document.write('<link rel="stylesheet" href="/assets/vendor/css/theme-default.css" />');
        winPrint.document.write('<link rel="stylesheet" href="/assets/css/main.css" />');
        winPrint.document.write("<body>");
        winPrint.document.write("<h5 class='text-center'>" + title + "</h1>");
        winPrint.document.body.appendChild(table);
        winPrint.document.close();
        winPrint.onload = function() {
            winPrint.focus();
            winPrint.print();
            winPrint.close();
        }
   
    });
}

// Keep all other existing functions for backward compatibility
// ... (rest of the existing functions remain unchanged)

